using PiiScanner.Application.Service;

namespace PiiScanner.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _services;

        public Worker(ILogger<Worker> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                using var scope = _services.CreateScope();
                var scanner = scope.ServiceProvider.GetRequiredService<ScanService>();

                await scanner.ScanAsync();

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }
}
