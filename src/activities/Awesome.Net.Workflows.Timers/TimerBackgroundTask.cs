using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Awesome.Net.Workflows.Timers
{
    public class TimerBackgroundTask : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TimerBackgroundTask> _logger;
        private readonly TimerBackgroundTaskOptions _options;
        public TimerBackgroundTask(IServiceProvider serviceProvider, ILogger<TimerBackgroundTask> logger, IOptions<TimerBackgroundTaskOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var workflowManager = _serviceProvider.GetRequiredService<IWorkflowManager>();
                    await workflowManager.TriggerEventAsync(nameof(TimerEvent));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception occurred while invoking workflows.");
                }

                await Task.Delay(_options.PollingTime, stoppingToken);
            }
        }
    }
}
