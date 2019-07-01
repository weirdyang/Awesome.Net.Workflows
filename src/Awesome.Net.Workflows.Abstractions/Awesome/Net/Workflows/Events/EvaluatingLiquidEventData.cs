using Awesome.Net.Workflows.Models;
using Fluid;

namespace Awesome.Net.Workflows.Events
{
    public class EvaluatingLiquidEventData
    {
        public EvaluatingLiquidEventData(WorkflowExecutionContext workflowExecutionContext, TemplateContext templateContext)
        {
            WorkflowExecutionContext = workflowExecutionContext;
            TemplateContext = templateContext;
        }

        public WorkflowExecutionContext WorkflowExecutionContext { get; }

        public TemplateContext TemplateContext { get; }
    }
}