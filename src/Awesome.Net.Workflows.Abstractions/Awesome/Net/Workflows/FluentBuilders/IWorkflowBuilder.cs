using System;
using System.Collections.Generic;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.FluentBuilders
{
    public interface IWorkflowBuilder
    {
        IActivityLibrary ActivityLibrary { get; }
        List<ActivityRecord> Activities { get; set; }
        List<Transition> Transitions { get; set; }
        IActivityBuilder StartWith<T>(Action<T> setup = null, string id = null) where T : IActivity;
        ActivityRecord BuildActivity<T>(T activity = default, Action<T> setup = null, string id = null,
            bool addToWorkflow = true)
            where T : IActivity;
        WorkflowType Build<T>() where T : IWorkflow, new();
        WorkflowType Build(IWorkflow workflow);
    }
}
