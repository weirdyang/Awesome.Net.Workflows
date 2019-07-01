using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Events
{
    public class WorkflowTypeCreatedEventData : WorkflowTypeEventData
    {
        public WorkflowTypeCreatedEventData(WorkflowType workflowType) : base(workflowType)
        {
        }
    }
}
