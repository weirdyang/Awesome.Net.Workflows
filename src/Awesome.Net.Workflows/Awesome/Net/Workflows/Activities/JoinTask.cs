using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;

namespace Awesome.Net.Workflows.Activities
{
    public class JoinTask : TaskActivity
    {
        public enum JoinMode
        {
            WaitAll,
            WaitAny
        }

        public override LocalizedString Category => T["Control Flow"];

        public JoinMode Mode
        {
            get => GetProperty(() => JoinMode.WaitAll);
            set => SetProperty(value);
        }

        private IList<string> Branches
        {
            get => GetProperty(() => new List<string>());
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Outcomes(T["Joined"]);
        }

        public override ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            var branches = Branches;
            var inboundTransitions = workflowContext.GetInboundTransitions(activityContext.ActivityRecord.ActivityId);
            var done = false;

            switch (Mode)
            {
                case JoinMode.WaitAll:
                    done = inboundTransitions.All(x => branches.Contains(GetTransitionKey(x)));
                    break;
                case JoinMode.WaitAny:
                    done = inboundTransitions.Any(x => branches.Contains(GetTransitionKey(x)));

                    if (done)
                    {
                        // Remove any inbound blocking activities.
                        var ancestorActivityIds = workflowContext
                            .GetInboundActivityPath(activityContext.ActivityRecord.ActivityId).ToList();
                        var blockingActivities = workflowContext.Workflow.BlockingActivities
                            .Where(x => ancestorActivityIds.Contains(x.ActivityId)).ToList();

                        foreach (var blockingActivity in blockingActivities)
                        {
                            workflowContext.Workflow.BlockingActivities.Remove(blockingActivity);
                        }
                    }

                    break;
            }

            if (done)
            {
                return Outcomes("Joined");
            }

            return Noop();
        }

        public override Task OnActivityExecutedAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            // Get outbound transitions of the executing activity.
            var outboundTransitions = workflowContext.GetOutboundTransitions(activityContext.ActivityRecord.ActivityId);

            // Get any transition that is pointing to this activity.
            var inboundTransitionsQuery =
                from transition in outboundTransitions
                let destinationActivity = workflowContext.GetActivity(transition.DestinationActivityId)
                where destinationActivity.Activity.TypeName == TypeName
                select transition;

            var inboundTransitions = inboundTransitionsQuery.ToList();

            foreach (var inboundTransition in inboundTransitions)
            {
                var mergeActivity =
                    (JoinTask) workflowContext.GetActivity(inboundTransition.DestinationActivityId).Activity;
                var branches = mergeActivity.Branches;
                mergeActivity.Branches =
                    branches.Union(new[] {GetTransitionKey(inboundTransition)}).Distinct().ToList();
            }

            return Task.CompletedTask;
        }

        private string GetTransitionKey(Transition transition)
        {
            var sourceActivityId = transition.SourceActivityId;
            var sourceOutcomeName = transition.SourceOutcomeName;

            return $"@{sourceActivityId}_{sourceOutcomeName}";
        }

        public JoinTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
