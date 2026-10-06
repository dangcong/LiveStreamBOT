using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Shared.Enums;
using BaseSource.Data.Entities;
using BaseSource.Shared.Classes;
using BaseSource.Shared.Interfaces;
using BaseSouce.Services.BotChatServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BaseSouce.Services.BackgroundServices
{
    public sealed class InactiveBotAlertService : BackgroundService
    {
        private static readonly TimeSpan DailyCheckTime = TimeSpan.FromHours(8);
        private static readonly TimeZoneInfo VietnamTimeZone = GetVietnamTimeZone();

        private readonly ILogger<InactiveBotAlertService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly string _stateFilePath;

        public InactiveBotAlertService(
            ILogger<InactiveBotAlertService> logger,
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;

            var configuredStateFilePath = configuration["InactiveBotAlertStatePath"];
            if (!string.IsNullOrWhiteSpace(configuredStateFilePath))
            {
                _stateFilePath = Path.IsPathRooted(configuredStateFilePath)
                    ? configuredStateFilePath
                    : Path.Combine(AppContext.BaseDirectory, configuredStateFilePath);
            }
            else
            {
                var localDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (string.IsNullOrWhiteSpace(localDataDirectory))
                {
                    localDataDirectory = AppContext.BaseDirectory;
                }

                _stateFilePath = Path.Combine(localDataDirectory, "LiveStreamBOT", "inactive-bot-alert-state.json");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var delay = GetDelayUntilNextCheck(DateTimeOffset.UtcNow, VietnamTimeZone);
                    var nextCheck = DateTimeOffset.UtcNow.Add(delay);
                    _logger.LogInformation(
                        "Next inactive BOT alert check scheduled at {NextCheck} ({TimeZoneId})",
                        TimeZoneInfo.ConvertTime(nextCheck, VietnamTimeZone),
                        VietnamTimeZone.Id);

                    await Task.Delay(delay, stoppingToken);
                    await CheckInactiveBotsAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to check inactive BOT alerts");
                }
            }
        }

        private static TimeZoneInfo GetVietnamTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
        }

        private static TimeSpan GetDelayUntilNextCheck(DateTimeOffset nowUtc, TimeZoneInfo timeZone)
        {
            var localNow = TimeZoneInfo.ConvertTime(nowUtc, timeZone);
            var nextLocalCheck = localNow.Date.Add(DailyCheckTime);
            if (nextLocalCheck <= localNow.DateTime)
            {
                nextLocalCheck = nextLocalCheck.AddDays(1);
            }

            var nextCheckUtc = TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(nextLocalCheck, DateTimeKind.Unspecified),
                timeZone);
            return new DateTimeOffset(nextCheckUtc, TimeSpan.Zero) - nowUtc;
        }

        private async Task CheckInactiveBotsAsync(CancellationToken cancellationToken)
        {
            var inactiveThreshold = _configuration.GetValue<int?>("InactiveBotDays");
            if (!inactiveThreshold.HasValue || inactiveThreshold.Value <= 0)
            {
                inactiveThreshold = 10;
            }

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BaseSourceDbContext>();
            var botChatService = scope.ServiceProvider.GetRequiredService<IBotChatService>();

            var bots = await dbContext.ManagerBOTs
                .Include(bot => bot.Manager)
                .Where(bot => bot.DeletedTime == null)
                .ToListAsync(cancellationToken);

            var alertedBotIds = await LoadAlertedBotIdsAsync(cancellationToken);
            if (bots.Count == 0)
            {
                if (alertedBotIds.Count > 0)
                {
                    alertedBotIds.Clear();
                    await SaveAlertedBotIdsAsync(alertedBotIds, cancellationToken);
                }

                return;
            }

            var botIds = bots.Select(bot => bot.Id).ToList();

            var lastPrimaryActivity = await dbContext.StreamHistorys
                .AsNoTracking()
                .Where(stream => stream.ManagerBOTId.HasValue && botIds.Contains(stream.ManagerBOTId.Value))
                .GroupBy(stream => stream.ManagerBOTId)
                .Select(group => new
                {
                    BotId = group.Key.Value,
                    LastActivity = group.Max(stream => stream.Updatedtime ?? stream.CreatedTime)
                })
                .ToDictionaryAsync(item => item.BotId, item => item.LastActivity, cancellationToken);

            var lastBackupActivity = await dbContext.StreamHistorys
                .AsNoTracking()
                .Where(stream => stream.ManagerBOTBackupId.HasValue && botIds.Contains(stream.ManagerBOTBackupId.Value))
                .GroupBy(stream => stream.ManagerBOTBackupId)
                .Select(group => new
                {
                    BotId = group.Key.Value,
                    LastActivity = group.Max(stream => stream.Updatedtime ?? stream.CreatedTime)
                })
                .ToDictionaryAsync(item => item.BotId, item => item.LastActivity, cancellationToken);

            var activePrimaryBotIds = await dbContext.StreamHistorys
                .AsNoTracking()
                .Where(stream => stream.ManagerBOTId.HasValue
                    && botIds.Contains(stream.ManagerBOTId.Value)
                    && stream.Status == StreamStatus.Streaming
                    && stream.EndTimeLive == null)
                .Select(stream => stream.ManagerBOTId.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            var activeBackupBotIds = await dbContext.StreamHistorys
                .AsNoTracking()
                .Where(stream => stream.ManagerBOTBackupId.HasValue
                    && botIds.Contains(stream.ManagerBOTBackupId.Value)
                    && stream.Status == StreamStatus.Streaming
                    && stream.EndTimeLive == null)
                .Select(stream => stream.ManagerBOTBackupId.Value)
                .Distinct()
                .ToListAsync(cancellationToken);

            var activeBotIds = activePrimaryBotIds.Concat(activeBackupBotIds).ToHashSet();
            var staffTelegram = await dbContext.Set<ConfigSystem>()
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            var now = DateTime.Now;
            var alertStateChanged = false;

            foreach (var bot in bots)
            {
                var lastActivity = bot.CreatedTime;
                if (lastPrimaryActivity.TryGetValue(bot.Id, out var primaryActivity) && primaryActivity > lastActivity)
                {
                    lastActivity = primaryActivity;
                }
                if (lastBackupActivity.TryGetValue(bot.Id, out var backupActivity) && backupActivity > lastActivity)
                {
                    lastActivity = backupActivity;
                }

                var inactiveDays = (int)(now.Date - lastActivity.Date).TotalDays;
                var shouldAlert = !activeBotIds.Contains(bot.Id) && inactiveDays > inactiveThreshold.Value;

                if (!shouldAlert)
                {
                    alertStateChanged |= alertedBotIds.Remove(bot.Id);
                    continue;
                }

                if (alertedBotIds.Contains(bot.Id))
                {
                    continue;
                }

                var recipients = new List<IBotChatId>();
                if (staffTelegram is not null)
                {
                    recipients.Add(staffTelegram);
                }
                if (bot.Manager is not null)
                {
                    recipients.Add(bot.Manager.GetBotChatIdForStaff());
                }

                var recipient = new BotChatId(recipients);
                var message = $"⚠️ BOT không hoạt động\n"
                    + $"BOT: {bot.Name}\n"
                    + $"Bot ID: {bot.BotId}\n"
                    + $"Nhóm: {bot.Group}\n"
                    + $"Manager: {bot.Manager?.UserName ?? "Chưa gán"}\n"
                    + $"Loại: {bot.LiveType?.ToString() ?? "Không rõ"}\n"
                    + $"Không hoạt động: {inactiveDays} ngày (ngưỡng {inactiveThreshold.Value} ngày)\n"
                    + $"Hoạt động gần nhất: {lastActivity:dd/MM/yyyy HH:mm:ss}";

                var delivered = await botChatService.SendMessageAndReportAsync(
                    recipient,
                    message,
                    BotChatService.SendType.Bot,
                    cancellationToken);

                if (delivered)
                {
                    alertedBotIds.Add(bot.Id);
                    await SaveAlertedBotIdsAsync(alertedBotIds, cancellationToken);
                    _logger.LogInformation(
                        "Inactive BOT alert delivered: ManagerBOTId={ManagerBOTId}, InactiveDays={InactiveDays}",
                        bot.Id,
                        inactiveDays);
                }
                else
                {
                    _logger.LogWarning(
                        "Inactive BOT alert not delivered; it will be retried: ManagerBOTId={ManagerBOTId}",
                        bot.Id);
                }
            }

            if (alertStateChanged)
            {
                await SaveAlertedBotIdsAsync(alertedBotIds, cancellationToken);
            }
        }

        private async Task<HashSet<int>> LoadAlertedBotIdsAsync(CancellationToken cancellationToken)
        {
            if (!File.Exists(_stateFilePath))
            {
                return new HashSet<int>();
            }

            await using var stateFile = File.OpenRead(_stateFilePath);
            var botIds = await JsonSerializer.DeserializeAsync<HashSet<int>>(stateFile, cancellationToken: cancellationToken);
            return botIds ?? new HashSet<int>();
        }

        private async Task SaveAlertedBotIdsAsync(HashSet<int> alertedBotIds, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(_stateFilePath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new InvalidOperationException("Inactive BOT alert state path must include a directory");
            }

            Directory.CreateDirectory(directory);
            var temporaryFilePath = $"{_stateFilePath}.{Guid.NewGuid():N}.tmp";
            try
            {
                await File.WriteAllTextAsync(
                    temporaryFilePath,
                    JsonSerializer.Serialize(alertedBotIds.OrderBy(id => id)),
                    cancellationToken);
                File.Move(temporaryFilePath, _stateFilePath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryFilePath))
                {
                    File.Delete(temporaryFilePath);
                }
            }
        }
    }
}
