using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Events
{
    public class WorkflowTypeDeletedEventData : WorkflowTypeEventData
    {
        public WorkflowTypeDeletedEventData(WorkflowType workflowType) : base(workflowType)
        {
        }
    }
}
