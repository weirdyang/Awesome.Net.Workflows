using System;
using System.Linq;
using Awesome.Net.Liquid;
using Awesome.Net.Scripting;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.FluentBuilders;
using Awesome.Net.Workflows.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Workflows
{
    public static class Startup
    {
        public static IServiceCollection AddWorkflow(this IServiceCollection services,
            Action<WorkflowOptions> setupAction = null)
        {
            if (services.Any(x => x.ServiceType == typeof(WorkflowOptions)))
            {
                throw new InvalidOperationException("Workflow services already registered");
            }

            var options = new WorkflowOptions(services);
            setupAction?.Invoke(options);
            options.AddDefaultWorkflowActivities();
            services.AddSingleton(options);

            services.AddLiquid();
            services.AddScripting();

            services.AddTransient<IActivityLibrary, ActivityLibrary>();
            services.AddTransient<IWorkflowTypeStore, MemoryWorkflowTypeStore>();
            services.AddTransient<IWorkflowStore, MemoryWorkflowStore>();
            services.AddTransient<IWorkflowManager, WorkflowManager>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();

            services.AddTransient<IWorkflowBuilder, WorkflowBuilder>();

            services.AddSingleton<ISecurityTokenService, SecurityTokenService>();
            services.AddSingleton<WorkflowExpressionEvaluator, WorkflowExpressionEvaluator>();
            services.AddSingleton<IExpressionEvaluator, JavaScriptExpressionEvaluator>();
            services.AddSingleton<IExpressionEvaluator, LiquidExpressionEvaluator>();

            services.RegisterWorkflowExecutionEventHandler<DefaultWorkflowExecutionEventHandler>();

            return services;
        }

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

        public static IServiceCollection
            RegisterWorkflowTypePersistenceEventHandler<T>(this IServiceCollection services)
            where T : class, IWorkflowTypePersistenceEventHandler
        {
            services.AddSingleton<IWorkflowTypePersistenceEventHandler, T>();
            return services;
        }

        private static void AddDefaultWorkflowActivities(this WorkflowOptions options)
        {
            // Primitives
            options.RegisterActivity<CorrelateTask>()
                .RegisterActivity<LogTask>()
                .RegisterActivity<SetOutputTask>()
                .RegisterActivity<SetPropertyTask>()
                .RegisterActivity<MissingActivity>();

            // Control Flow
            options.RegisterActivity<IfElseTask>()
                .RegisterActivity<ForEachTask>()
                .RegisterActivity<ForkTask>()
                .RegisterActivity<JoinTask>()
                .RegisterActivity<ScriptTask>()
                .RegisterActivity<WhileLoopTask>();
        }
    }
}
