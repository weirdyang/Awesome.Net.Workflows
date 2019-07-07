using System;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.FluentBuilders
{
    public interface IActivityBuilder
    {
        IWorkflowBuilder WorkflowBuilder { get; }
        ActivityRecord CurrentActivity { get; }
        void Connect(string targetId, string outcome = "Done");
        void Connect(Func<string> targetIdFunc, string outcome = "Done");
        IActivityBuilder When(bool outcome);
        IActivityBuilder When(string outcome = "Done");
        IActivityBuilder Then<TNext>(Action<TNext> setup = null, Action<IActivityBuilder> branch = null,
            string id = null) where TNext : IActivity;
        IActivityBuilder Add<TActivity>(string id, Action<TActivity> setup = null) where TActivity : IActivity;
        IParallelActivityBuilder Fork(string id = null);
    }
}
