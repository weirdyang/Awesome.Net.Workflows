using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Contexts
{
    public class WorkflowContext
    {
        public WorkflowContext(Workflow workflow)
        {
            Workflow = workflow;
        }

        public Workflow Workflow { get; }
    }
}
