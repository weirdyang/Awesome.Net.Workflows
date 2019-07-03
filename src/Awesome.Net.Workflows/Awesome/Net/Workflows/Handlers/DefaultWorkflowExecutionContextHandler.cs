using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Scripting;

namespace Awesome.Net.Workflows.Handlers
{
    public class DefaultWorkflowExecutionContextHandler : IWorkflowExecutionContextHandler
    {
        public Task EvaluatingLiquidAsync(WorkflowExecutionLiquidContext context)
        {
            return Task.CompletedTask;
        }

        public Task EvaluatingScriptAsync(WorkflowExecutionScriptContext context)
        {
            context.ScopedMethodProviders.Add(new WorkflowMethodsProvider(context.WorkflowContext));
            return Task.CompletedTask;
        }
    }
}
