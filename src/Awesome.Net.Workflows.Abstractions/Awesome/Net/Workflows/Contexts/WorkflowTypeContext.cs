using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Contexts
{
    public class WorkflowTypeContext
    {
        public WorkflowTypeContext(WorkflowType workflowType)
        {
            WorkflowType = workflowType;
        }

        public WorkflowType WorkflowType { get; }
    }
}
