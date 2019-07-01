using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Events
{
    public class WorkflowTypeEventData
    {
        public WorkflowTypeEventData(WorkflowType workflowType)
        {
            WorkflowType = workflowType;
        }

        public WorkflowType WorkflowType { get; }
    }
}
