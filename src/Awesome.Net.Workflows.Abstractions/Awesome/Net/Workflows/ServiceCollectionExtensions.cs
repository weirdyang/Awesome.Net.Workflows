using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Workflows
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterWorkflowExecutionEventHandler<T>(this IServiceCollection services)
            where T : class, IWorkflowExecutionEventHandler
        {
            services.AddSingleton<IWorkflowExecutionEventHandler, T>();
            return services;
        }

        public static IServiceCollection RegisterWorkflowPersistenceHandler<T>(this IServiceCollection services)
            where T : class, IWorkflowPersistenceHandler
        {
            services.AddSingleton<IWorkflowPersistenceHandler, T>();
            return services;
        }

        public static IServiceCollection RegisterWorkflowTypePersistenceEventHandler<T>(this IServiceCollection services)
            where T : class, IWorkflowTypePersistenceEventHandler
        {
            services.AddSingleton<IWorkflowTypePersistenceEventHandler, T>();
            return services;
        }

        public static IServiceCollection AddWorkflowActivity<T>(this IServiceCollection services) where T : IActivity
        {
            var sp = services.BuildServiceProvider();
            var options = sp.GetService<WorkflowOptions>();
            options.AddActivity<T>();

            return services;
        }
    }
}
