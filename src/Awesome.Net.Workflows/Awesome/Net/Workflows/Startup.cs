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
        public static IServiceCollection ConfigureWorkflows(this IServiceCollection services, Action<WorkflowOptions> setupAction = null)
        {
            WorkflowOptions options;
            if (services.Any(x => x.ServiceType == typeof(WorkflowOptions)))
            {
                var sp = services.BuildServiceProvider();
                options = sp.GetService<WorkflowOptions>();
                setupAction?.Invoke(options);
            }
            else
            {
                services.AddLogging();
                services.AddLocalization();

                services.ConfigureLiquid();
                services.ConfigureScripting();

                options = new WorkflowOptions(services);
                setupAction?.Invoke(options);
                options.AddDefaultWorkflowActivities();
                services.AddSingleton(options);

                services.AddTransient<IActivityLibrary, ActivityLibrary>();
                services.AddTransient<IWorkflowManager, WorkflowManager>();
                services.AddTransient<IDateTimeProvider, DateTimeProvider>();
                services.AddTransient<IWorkflowBuilder, WorkflowBuilder>();

                services.AddSingleton<IWorkflowTypeStore, MemoryWorkflowTypeStore>();
                services.AddSingleton<IWorkflowStore, MemoryWorkflowStore>();
                services.AddSingleton<ISecurityTokenService, SecurityTokenService>();
                services.AddSingleton<IWorkflowExpressionEvaluator, WorkflowExpressionEvaluator>();
                services.AddSingleton<IExpressionEvaluator, JavaScriptExpressionEvaluator>();
                services.AddSingleton<IExpressionEvaluator, LiquidExpressionEvaluator>();
                services.AddSingleton<IWorkflowTypeLoader, WorkflowTypeLoader>();

                services.RegisterWorkflowExecutionEventHandler<DefaultWorkflowExecutionEventHandler>();
            }

            return services;
        }

        private static void AddDefaultWorkflowActivities(this WorkflowOptions options)
        {
            // Primitives
            options.AddActivity<CorrelateTask>()
                .AddActivity<LogTask>()
                .AddActivity<SetOutputTask>()
                .AddActivity<SetPropertyTask>();

            // Control Flow
            options.AddActivity<IfElseTask>()
                .AddActivity<ForEachTask>()
                .AddActivity<ForkTask>()
                .AddActivity<JoinTask>()
                .AddActivity<ScriptTask>()
                .AddActivity<SwitchTask>()
                .AddActivity<WhileLoopTask>();
        }
    }
}
