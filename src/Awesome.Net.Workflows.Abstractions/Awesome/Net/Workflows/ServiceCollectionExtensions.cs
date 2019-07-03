using Awesome.Net.Workflows.Activities;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Workflows
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowActivity<T>(this IServiceCollection services)
            where T : class, IActivity
        {
            services.AddScoped<T>();

            services.Configure<WorkflowOptions>(options => options.RegisterActivity<T>());

            return services;
        }
    }
}
