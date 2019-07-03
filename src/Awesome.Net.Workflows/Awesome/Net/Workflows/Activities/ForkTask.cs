using System;
using System.Collections.Generic;
using System.Linq;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;

namespace Awesome.Net.Workflows.Activities
{
    public class ForkTask : TaskActivity
    {
        public override LocalizedString Category => T["Control Flow"];

        public IList<string> Forks
        {
            get => GetProperty(() => new List<string>());
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Forks.Select(x => Outcome(T[x]));
        }

        public override ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Outcomes(Forks);
        }

        public ForkTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
