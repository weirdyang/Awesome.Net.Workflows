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

        public IParallelActivityBuilder Branch(Action<IActivityBuilder> branchBuilder, string branchName = null)
        {
            var branches = new List<string>();
            if (ActivityBuilder.CurrentActivity.Properties.ContainsKey("Forks"))
            {
                branches = ActivityBuilder.CurrentActivity.Properties["Forks"].ToObject<List<string>>();
            }

            if (branchName.IsNullOrWhiteSpace())
            {
                branchName = $"Branch_{RandomHelper.Generate26UniqueId().Left(6)}";
            }
            else
            {
                if (branches.Any(x => x == branchName))
                {
                    throw new ArgumentException($"Branch: {branchName} already exists.", nameof(branchName));
                }
            }

            branches.Add(branchName.Trim());

            ActivityBuilder.CurrentActivity.Properties["Forks"] = JToken.FromObject(branches);

            ActivityBuilder.When(branchName);

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
