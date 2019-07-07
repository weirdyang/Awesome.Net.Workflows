using System;
using System.Threading.Tasks;
using Awesome.Net.Workflows;
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
                .AddWorkflowActivity<ConsoleWriteLineTask>()
                .BuildServiceProvider();

            var workflowManager = services.GetService<IWorkflowManager>();
            var ctx = await workflowManager.StartWorkflowAsync<Sample01Workflow>();
            var json = JsonConvert.SerializeObject(ctx.WorkflowType);

            Console.ReadLine();
        }
    }
}
