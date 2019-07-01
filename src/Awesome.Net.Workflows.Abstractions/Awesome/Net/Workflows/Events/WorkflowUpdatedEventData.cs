using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Events
{
    public class WorkflowUpdatedEventData : WorkflowEventData
    {
        public WorkflowUpdatedEventData(Workflow workflow) : base(workflow)
        {
        }
    }
}
