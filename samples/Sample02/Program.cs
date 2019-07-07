using System;
using System.Threading.Tasks;
using Awesome.Net.Workflows;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.FluentBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace Sample02
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection()
                .ConfigureWorkflows(o => o.AddActivity<ConsoleWriteLineTask>())
                .BuildServiceProvider();

            var workflowBuilder = services.GetService<IWorkflowBuilder>();
            var workflowType = workflowBuilder.StartWith<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("StartWith ConsoleWriteLineTask"))
                .Then<SetPropertyTask>(x =>
                {
                    x.PropertyName = "Value";
                    x.Value = new JavaScriptExpr<object>("'Awesome!'");
                })
                .Then<ConsoleWriteLineTask>(x => x.Text = new LiquidExpr("Value has set: {{Workflow.Properties['Value']}}"))
                .Build("DemoWorkflow2");

            var workflowManager = services.GetService<IWorkflowManager>();
            await workflowManager.StartWorkflowAsync(workflowType);

            Console.ReadLine();
        }
    }
}
