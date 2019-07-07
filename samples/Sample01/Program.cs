using System;
using System.Threading.Tasks;
using Awesome.Net.Workflows;
using Awesome.Net.Workflows.FluentBuilders;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Sample01
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection()
                .ConfigureWorkflows()
                //.AddWorkflowActivity<ConsoleWriteLineTask>()
                .ConfigureWorkflows(o => o.AddActivity<ConsoleWriteLineTask>())
                .BuildServiceProvider();

            var workflowManager = services.GetService<IWorkflowManager>();
            var workflowBuilder = services.GetService<IWorkflowBuilder>();
            await workflowManager.StartWorkflowAsync<Sample01Workflow>();

            var workflowType = workflowBuilder.Build<Sample01Workflow>();
            var json = JsonConvert.SerializeObject(workflowType);

            Console.ReadLine();
        }
    }
}
