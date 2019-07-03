using System;
using Awesome.Net.Workflows.Contexts;

namespace Awesome.Net.Workflows.Activities
{
    public abstract class EventActivity : Activity, IEvent
    {
        public override ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext, ActivityExecutionContext activityContext)
        {
            // Halt the workflow to wait for the event to occur.
            return Halt();
        }

        protected EventActivity(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}