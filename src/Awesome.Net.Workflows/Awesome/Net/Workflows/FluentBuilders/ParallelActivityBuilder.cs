using System;
using System.Collections.Generic;
using System.Linq;
using Awesome.Net.Workflows.Activities;
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

            IList<string> branches;
            if (ActivityBuilder.CurrentActivity.Properties.ContainsKey("Forks"))
            {
                branches = ActivityBuilder.CurrentActivity.Properties["Forks"].ToObject<IList<string>>();

                if (branches.Any(x => x == branch))
                {
                    throw new ArgumentException($"Branch: {branch} already exists.", nameof(branch));
                }
            }
            else
            {
                branches = new List<string>();
            }

            branches.Add(branch.Trim());
            ActivityBuilder.CurrentActivity.Properties["Forks"] = JToken.FromObject(branches);

            ActivityBuilder.When(branch);

            branchBuilder.Invoke(ActivityBuilder);

            return this;
        }

        public IActivityBuilder Join(string id, bool waitAll = true)
        {
            var joinTask = (JoinTask)ActivityBuilder.WorkflowBuilder.ActivityLibrary.GetActivityByName(typeof(JoinTask).Name);
            joinTask.Mode = waitAll ? JoinTask.JoinMode.WaitAll : JoinTask.JoinMode.WaitAny;
            var joinActivity = ActivityBuilder.WorkflowBuilder.BuildActivity(joinTask, id: id);
            var activityBuilder = new ActivityBuilder(ActivityBuilder.WorkflowBuilder, joinActivity);
            activityBuilder.When("Joined");
            return activityBuilder;
        }
    }
}
