using Awesome.Net.Liquid;
using Awesome.Net.Scripting;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Workflows
{
    public static class Startup
    {
        public static IServiceCollection ConfigureWorkflows(this IServiceCollection services)
        {
            services.ConfigureLiquid();
            services.ConfigureScripting();

            services.AddSingleton<ISecurityTokenService, SecurityTokenService>();
            services.AddScoped<IActivityLibrary, ActivityLibrary>();
            services.AddScoped<IWorkflowTypeStore, MemoryWorkflowTypeStore>();
            services.AddScoped<IWorkflowStore, MemoryWorkflowStore>();
            services.AddScoped<IWorkflowManager, WorkflowManager>();
            services.AddWorkflowExecutionContextHandler<DefaultWorkflowExecutionContextHandler>();

            services.AddScoped<IWorkflowExpressionEvaluator, WorkflowExpressionEvaluator>();
            services.AddExpressionEvaluator<JavaScriptExpressionEvaluator>();
            services.AddExpressionEvaluator<LiquidExpressionEvaluator>();

            services.AddDefaultActivities();

            return services;
        }

        public static IServiceCollection AddWorkflowExecutionContextHandler<T>(this IServiceCollection services)
            where T : class, IWorkflowExecutionContextHandler
        {
            services.AddScoped<IWorkflowExecutionContextHandler, T>();
            return services;
        }

        public static IServiceCollection AddExpressionEvaluator<T>(this IServiceCollection services)
            where T : class, IExpressionEvaluator
        {
            services.AddScoped<IExpressionEvaluator, T>();
            return services;
        }

        private static void AddDefaultActivities(this IServiceCollection services)
        {
            services.AddActivity<CorrelateTask>()
                .AddActivity<LogTask>()
                .AddActivity<SetOutputTask>()
                .AddActivity<SetPropertyTask>()
                .AddActivity<MissingActivity>();

            services.AddActivity<IfElseTask>()
                .AddActivity<ForEachTask>()
                .AddActivity<ForkTask>()
                .AddActivity<JoinTask>()
                .AddActivity<ScriptTask>()
                .AddActivity<WhileLoopTask>();
        }
    }
}