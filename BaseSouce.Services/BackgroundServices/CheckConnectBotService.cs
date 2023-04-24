using BaseSouce.Data.EF;
using BaseSouce.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Services.BackgroundServices
{
    public class CheckConnectBotService : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly ILogger<CheckConnectBotService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public CheckConnectBotService(ILogger<CheckConnectBotService> logger, IServiceScopeFactory scopeFactory)
        {

            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Order History Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Order History Service stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using var _db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<BaseSourceDbContext>();

            var bots = await _db.ManagerBOTs.ToListAsync();
            var timeCurrent = DateTime.UtcNow.AddSeconds(-15);
            foreach (var bot in bots)
            {
                if (bot.UpdatedTime == null || timeCurrent > bot.UpdatedTime)
                {
                    bot.Status = ManagerBOTStatus.Disconnected;

                    var streamsCurrents = _db.StreamHistorys.Where(x => x.ManagerBOTId == bot.Id).ToList();
                    if (streamsCurrents != null && streamsCurrents.Any())
                    {
                        foreach (var item in streamsCurrents)
                        {
                            item.Status = StreamStatus.Stop;
                        }
                    }
                    await _db.SaveChangesAsync();
                }
            }

        }

    }
}
