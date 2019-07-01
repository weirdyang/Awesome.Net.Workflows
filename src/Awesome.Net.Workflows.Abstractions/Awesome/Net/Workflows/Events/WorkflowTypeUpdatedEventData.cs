using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Events
{
    public class WorkflowTypeUpdatedEventData : WorkflowTypeEventData
    {
        public WorkflowTypeUpdatedEventData(WorkflowType workflowType) : base(workflowType)
        {
        }
    }
}
