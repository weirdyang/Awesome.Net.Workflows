using System;
using System.Collections.Generic;
using System.Linq;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Models;
using Newtonsoft.Json.Linq;

namespace Awesome.Net.Workflows.FluentBuilders
{
    public class ParallelActivityBuilder : IParallelActivityBuilder
    {
        public IActivityBuilder ActivityBuilder { get; }

        public ParallelActivityBuilder(IActivityBuilder activityBuilder)
        {
            ActivityBuilder = activityBuilder;
        }

        public IParallelActivityBuilder Do(string branch, Action<IActivityBuilder> branchBuilder)
        {
            if (branch == null) throw new ArgumentNullException(nameof(branch));
            var branches = ActivityBuilder.CurrentActivity.Properties["Forks"].ToObject<IList<string>>();
            if (branches.Any(x => x == branch))
            {
                throw new ArgumentException($"Branch: {branch} already exists.", nameof(branch));
            }

            branches.Add(branch.Trim());
            ActivityBuilder.CurrentActivity.Properties["Forks"] = new JObject(branches);

            ActivityBuilder.When(branch);

            branchBuilder.Invoke(ActivityBuilder);

            return this;
        }

        public IActivityBuilder Join(bool waitAll = true, params string[] branches)
        {
            var branchList = ActivityBuilder.CurrentActivity.Properties["Forks"].ToObject<IList<string>>();
            if (branchList.Count < 2)
            {
                throw new ArgumentException($"Fork branch count must be greater than 2.", nameof(branchList));
            }

            branchList = branchList.WhereIf(!branches.IsNullOrEmpty(), x => branches.Any(b => b.Trim() == x)).Distinct()
                .ToList();

            if (branchList.Count() < 2)
            {
                throw new ArgumentException($"Join branch count must be greater than 2.", nameof(branches));
            }

            var joinTask =
                (JoinTask) ActivityBuilder.WorkflowBuilder.ActivityLibrary.GetActivityByName(typeof(JoinTask).Name);
            joinTask.Mode = waitAll ? JoinTask.JoinMode.WaitAll : JoinTask.JoinMode.WaitAny;
            var joinActivity = ActivityBuilder.WorkflowBuilder.BuildActivity(joinTask);

            var branchLastActivities = GetBranchLastActivities(ActivityBuilder.CurrentActivity, branchList);

            foreach (var activity in branchLastActivities)
            {
                var transition = new Transition
                {
                    Id = Guid.NewGuid(),
                    SourceActivityId = activity.Id,
                    DestinationActivityId = joinActivity.Id,
                    SourceOutcomeName = "Done"
                };
                ActivityBuilder.WorkflowBuilder.Transitions.Add(transition);
            }

            var activityBuilder = new ActivityBuilder(ActivityBuilder.WorkflowBuilder, joinActivity);
            activityBuilder.When("Joined");
            return activityBuilder;
        }

        private IEnumerable<ActivityRecord> GetBranchLastActivities(ActivityRecord sourceActivity,
            ICollection<string> branches)
        {
            var lastBranchActivities = new List<ActivityRecord>();
            var nextTransitions = ActivityBuilder.WorkflowBuilder.Transitions
                .WhereIf(branches.Any(), x => branches.Contains(x.SourceOutcomeName))
                .Where(x => x.SourceActivityId == sourceActivity.Id).ToList();

            if (!nextTransitions.Any())
            {
                lastBranchActivities.Add(sourceActivity);
            }

            foreach (var transition in nextTransitions)
            {
                var nextActivity =
                    ActivityBuilder.WorkflowBuilder.Activities.First(x => x.Id == transition.DestinationActivityId);
                lastBranchActivities.AddRange(GetBranchLastActivities(nextActivity, branches));
            }

            return lastBranchActivities;
        }
    }
}
