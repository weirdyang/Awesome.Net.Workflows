using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Activities
{
    public abstract class EventActivity : Activity, IEvent
    {
        public override ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            // Halt the workflow to wait for the event to occur.
            return Halt();
        }
    }
}