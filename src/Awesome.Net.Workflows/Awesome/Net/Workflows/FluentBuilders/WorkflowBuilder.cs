using System;
using System.Collections.Generic;
using System.Linq;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.FluentBuilders
{
    public class WorkflowBuilder : IWorkflowBuilder
    {
        public IActivityLibrary ActivityLibrary { get; }
        public List<ActivityRecord> Activities { get; set; } = new List<ActivityRecord>();
        public List<Transition> Transitions { get; set; } = new List<Transition>();

        public WorkflowBuilder(IActivityLibrary activityLibrary)
        {
            ActivityLibrary = activityLibrary;
        }

        public IActivityBuilder StartWith<T>(Action<T> setup = null, string id = null) where T : IActivity
        {
            var activityRecord = BuildActivity(default, setup, id);
            activityRecord.IsStart = true;
            var activityBuilder = new ActivityBuilder(this, activityRecord);
            return activityBuilder;
        }

        public ActivityRecord BuildActivity<T>(T activity = default, Action<T> setup = null, string id = null,
            bool addToWorkflow = true) where T : IActivity
        {
            activity = activity == null ? (T)ActivityLibrary.GetActivityByName(typeof(T).Name) : activity;
            setup?.Invoke(activity);
            var activityRecord = ActivityRecord.FromActivity(activity);
            if (id.IsNullOrWhiteSpace())
            {
                activityRecord.ActivityId = $"{RandomHelper.Generate26UniqueId()}";
            }
            else
            {
                var e = Activities.FirstOrDefault(x => x.ActivityId == id);
                if (e != null)
                {
                    throw new ArgumentException($"ActivityId: {id} already exists.", nameof(id));
                }
                else
                {
                    activityRecord.ActivityId = id;
                }
            }

            if (addToWorkflow)
            {
                Activities.Add(activityRecord);
            }

            return activityRecord;
        }

        public WorkflowType Build<T>() where T : IWorkflow, new()
        {
            var workflow = new T();
            workflow.Build(this);
            return Build(workflow);
        }

        public WorkflowType Build(IWorkflow workflow)
        {
            var workflowType = new WorkflowType()
            {
                Id = workflow.Id,
                WorkflowTypeId = workflow.WorkflowTypeId,
                Name = workflow.Name,
                IsEnabled = workflow.IsEnabled,
                IsSingleton = workflow.IsSingleton,
                DeleteFinishedWorkflows = workflow.DeleteFinishedWorkflows,
                Activities = Activities,
                Transitions = CleanupTransitions()
            };

            return workflowType;
        }

        // Remove invalid transitions
        private List<Transition> CleanupTransitions()
        {
            var transitions = Transitions.Where(x => !x.DestinationActivityId.IsNullOrEmpty());
            transitions = transitions.Where(transition => Activities.All(x => x.ActivityId == transition.DestinationActivityId)).ToList();
            return transitions.ToList();
        }
    }
}
