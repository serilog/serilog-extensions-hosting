using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleServiceSample
{
    public class PrintTimeService : BackgroundService
    {
        private readonly ILogger _logger;

        public PrintTimeService(ILogger<PrintTimeService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("The current time is: {CurrentTime}", DateTimeOffset.UtcNow);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
