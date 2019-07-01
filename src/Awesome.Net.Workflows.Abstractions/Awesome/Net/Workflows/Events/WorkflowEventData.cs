using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Events
{
    public class WorkflowEventData
    {
        public WorkflowEventData(Workflow workflow)
        {
            Workflow = workflow;
        }

        public Workflow Workflow { get; }
    }
}
