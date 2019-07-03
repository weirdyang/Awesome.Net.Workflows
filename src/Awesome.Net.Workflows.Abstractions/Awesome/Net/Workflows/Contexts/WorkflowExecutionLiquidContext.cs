using Fluid;

namespace Awesome.Net.Workflows.Contexts
{
    public class WorkflowExecutionLiquidContext
    {
        public WorkflowExecutionLiquidContext(
            TemplateContext templateContext,
            WorkflowExecutionContext workflowContext)
        {
            WorkflowContext = workflowContext;
            TemplateContext = templateContext;
        }

        public WorkflowExecutionContext WorkflowContext { get; }
        public TemplateContext TemplateContext { get; }
    }
}
