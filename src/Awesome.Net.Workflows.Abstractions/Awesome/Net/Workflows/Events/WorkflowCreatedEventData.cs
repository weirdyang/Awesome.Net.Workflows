using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Events
{
    public class WorkflowCreatedEventData : WorkflowEventData
    {
        public WorkflowCreatedEventData(Workflow workflow) : base(workflow)
        {
        }
    }
}
