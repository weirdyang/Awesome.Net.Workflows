using System;
using System.Threading.Tasks;
using Awesome.Net.Workflows;
using Awesome.Net.Workflows.Models;
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

            Console.ReadLine();
        }
    }
}
