using BaseSouce.Data.EF;
using BaseSouce.Services.Services.Stream;
using BaseSouce.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BaseSouce.Services.BackgroundServices
{
    public class CheckLiveBackupService : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly ILogger<CheckConnectBotService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public CheckLiveBackupService(ILogger<CheckConnectBotService> logger, IServiceScopeFactory scopeFactory)
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
            _logger.LogInformation("Timed Hosted Check Live Backup Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Check Live Backup Service stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
           // _logger.LogInformation("Begin Check Live Backup.");
            var _streamService = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IStreamClientService>();

            await _streamService.CheckLiveBackupAsync();
            //_logger.LogInformation("End Check Live Backup.");

        }
    }
}
