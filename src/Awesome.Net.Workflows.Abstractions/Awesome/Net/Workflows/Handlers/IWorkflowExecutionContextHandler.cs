using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;

namespace Awesome.Net.Workflows.Handlers
{
    public interface IWorkflowExecutionContextHandler
    {
        Task EvaluatingLiquidAsync(WorkflowExecutionLiquidContext context);
        Task EvaluatingScriptAsync(WorkflowExecutionScriptContext context);
    }
}
