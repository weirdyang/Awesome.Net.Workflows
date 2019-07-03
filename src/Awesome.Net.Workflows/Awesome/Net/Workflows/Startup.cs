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
            services.AddScoped<IExpressionEvaluator, JavaScriptExpressionEvaluator>();
            services.AddScoped<IExpressionEvaluator, LiquidExpressionEvaluator>();

            services.AddDefaultWorkflowActivities();

            return services;
        }

        public static IServiceCollection AddWorkflowExecutionContextHandler<T>(this IServiceCollection services)
            where T : class, IWorkflowExecutionContextHandler
        {
            services.AddScoped<IWorkflowExecutionContextHandler, T>();
            return services;
        }

        private static void AddDefaultWorkflowActivities(this IServiceCollection services)
        {
            services.AddWorkflowActivity<CorrelateTask>()
                .AddWorkflowActivity<LogTask>()
                .AddWorkflowActivity<SetOutputTask>()
                .AddWorkflowActivity<SetPropertyTask>()
                .AddWorkflowActivity<MissingActivity>();

            services.AddWorkflowActivity<IfElseTask>()
                .AddWorkflowActivity<ForEachTask>()
                .AddWorkflowActivity<ForkTask>()
                .AddWorkflowActivity<JoinTask>()
                .AddWorkflowActivity<ScriptTask>()
                .AddWorkflowActivity<WhileLoopTask>();
        }
    }
}
