using System.Collections.Generic;
using System.Threading.Tasks;
using Awesome.Net.Data;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace Awesome.Net.Workflows.Activities
{
    public class CorrelateTask : TaskActivity, ITransientDependency
    {
        public override LocalizedString Category => L["Primitives"];

        public IWorkflowExpression<string> Value
        {
            get => this.GetProperty(() => new JavaScriptExpression<string>());
            set => this.SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(L["Done"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            var value = (await ExpressionEvaluator.EvaluateAsync(Value, workflowContext))?.Trim();
            workflowContext.CorrelationId = value;

            return Outcomes("Done");
        }
    }
}