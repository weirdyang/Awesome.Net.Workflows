using System;
using System.Linq;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.FluentBuilders
{
    public class ActivityBuilder : IActivityBuilder
    {
        public ActivityBuilder(IWorkflowBuilder workflowBuilder, ActivityRecord currentActivity)
        {
            WorkflowBuilder = workflowBuilder;
            CurrentActivity = currentActivity;
        }

        public IWorkflowBuilder WorkflowBuilder { get; }

        public ActivityRecord CurrentActivity { get; }

        public IActivityBuilder Connect(string targetId, string outcome = "Done")
        {
            var transition = WorkflowBuilder.Transitions.FirstOrDefault(x => x.DestinationActivityId.IsNullOrEmpty());
            if (transition == null)
            {
                When(outcome);
            }

            transition = WorkflowBuilder.Transitions.First(x => x.DestinationActivityId.IsNullOrEmpty());
            transition.DestinationActivityId = targetId;

            return this;
        }

        public IActivityBuilder Connect(Func<string> targetIdFunc, string outcome = "Done")
        {
            return Connect(targetIdFunc(), outcome);
        }

        public IActivityBuilder When(bool outcome)
        {
            return When(outcome.ToString());
        }

        public IActivityBuilder When(string outcome = "Done")
        {
            if (outcome == null) throw new ArgumentNullException(nameof(outcome));
            if (WorkflowBuilder.Transitions.Any(x => x.DestinationActivityId.IsNullOrEmpty()))
            {
                throw new InvalidOperationException($"The 'When' method cannot be followed another 'When' method.");
            }

            var transition = new Transition
            {
                Id = Guid.NewGuid(),
                SourceActivityId = CurrentActivity.Id,
                DestinationActivityId = null,
                SourceOutcomeName = outcome
            };
            WorkflowBuilder.Transitions.Add(transition);
            return this;
        }

        public IActivityBuilder Then<TNext>(Action<TNext> setup = null, Action<IActivityBuilder> branch = null,
            string id = null) where TNext : IActivity
        {
            var next = WorkflowBuilder.BuildActivity(default, setup, id);

            Connect(next.Id);

            var activityBuilder = new ActivityBuilder(WorkflowBuilder, next);
            branch?.Invoke(activityBuilder);

            return activityBuilder;
        }

        public IParallelActivityBuilder Fork()
        {
            var forkTask = (ForkTask) WorkflowBuilder.ActivityLibrary.GetActivityByName(typeof(ForkTask).Name);
            var forkActivity = WorkflowBuilder.BuildActivity(forkTask);

            Connect(forkActivity.Id);

            var activityBuilder = new ActivityBuilder(WorkflowBuilder, forkActivity);
            var builder = new ParallelActivityBuilder(activityBuilder);
            return builder;
        }
    }
}
