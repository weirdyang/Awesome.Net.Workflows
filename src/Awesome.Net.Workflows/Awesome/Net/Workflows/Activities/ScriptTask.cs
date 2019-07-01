using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awesome.Net.Data;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Awesome.Net.Workflows.Scripting;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace Awesome.Net.Workflows.Activities
{
    public class ScriptTask : TaskActivity, ITransientDependency
    {
        public override LocalizedString Category => L["ControlFlow"];

        public IList<string> AvailableOutcomes
        {
            get => this.GetProperty(() => new List<string> { "Done" });
            set => this.SetProperty(value);
        }

        /// <summary>
        /// The script can call any available functions, including setOutcome().
        /// </summary>
        public IWorkflowExpression<object> Script
        {
            get => this.GetProperty(() => new JavaScriptExpression<object>("setOutcome('Done');"));
            set => this.SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(AvailableOutcomes.Select(x => L[x]).ToArray());
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            var outcomes = new List<string>();
            await ExpressionEvaluator.EvaluateAsync(Script, workflowContext, default, new OutcomeMethodProvider(outcomes));
            return Outcomes(outcomes);
        }
    }
}