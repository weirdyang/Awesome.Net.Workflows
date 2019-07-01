using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Events
{
    public class WorkflowDeletedEventData : WorkflowEventData
    {
        public WorkflowDeletedEventData(Workflow workflow) : base(workflow)
        {
        }
    }
}
