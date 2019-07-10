using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Awesome.Net.Workflows.Timers
{
    public static class Startup
    {
        public static IServiceCollection AddTimerActivity(this IServiceCollection services, Action<OptionsBuilder<TimerBackgroundTaskOptions>> options = null)
        {
            var optionsBuilder = services.AddOptions<TimerBackgroundTaskOptions>();
            options?.Invoke(optionsBuilder);
            services.AddOptions();
            services.AddHostedService<TimerBackgroundTask>();
            services.AddWorkflowActivity<TimerEvent>();

            return services;
        }
    }
}
