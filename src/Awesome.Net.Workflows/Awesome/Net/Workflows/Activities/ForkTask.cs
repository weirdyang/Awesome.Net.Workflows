using System.Collections.Generic;
using System.Linq;
using Awesome.Net.Data;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace Awesome.Net.Workflows.Activities
{
    public class ForkTask : TaskActivity, ITransientDependency
    {
        public override LocalizedString Category => L["ControlFlow"];

        public IList<string> Forks
        {
            get => this.GetProperty(() => new List<string>());
            set => this.SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Forks.Select(x => Outcome(L[x]));
        }

        public override ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(Forks);
        }
    }
}