using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebSiteStatus
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private HttpClient _client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            _logger.LogInformation("The service has been stoped..!");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var result = _client.GetAsync("http://services/intergartion/api/values").Result;
                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"The web site is up. Status code : { result.StatusCode}");

                }
                else 
                {
                    _logger.LogError($"The web site is down. Status code : { result.StatusCode}");
                }   
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
