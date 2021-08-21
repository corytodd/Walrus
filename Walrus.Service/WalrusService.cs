using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Walrus.Service
{

    public class WalrusService : BackgroundService
    {
        private readonly ILogger<WalrusService> _logger;
        private ServiceStates _serviceState;

        public WalrusService(ILogger<WalrusService> logger)
        {
            _logger = logger;
            _serviceState = ServiceStates.Stopped;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _serviceState = ServiceStates.Starting;

            _logger.LogInformation("Worker {state} at: {time}", _serviceState, DateTimeOffset.Now);

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _serviceState = ServiceStates.Stopping;

            _logger.LogInformation("Worker {state} at: {time}", _serviceState, DateTimeOffset.Now);

            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(ShuttingDown);

            _serviceState = ServiceStates.Running;

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker {} running at: {time}", _serviceState, DateTimeOffset.Now);
                try
                {
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        private void ShuttingDown()
        {
            _serviceState = ServiceStates.Stopped;

            _logger.LogInformation("Worker {state} at: {time}", _serviceState, DateTimeOffset.Now);
        }
    }
}