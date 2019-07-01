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
    public class IfElseTask : TaskActivity, ITransientDependency
    {
        public override LocalizedString Category => L["ControlFlow"];

        /// <summary>
        /// A script evaluating to either true or false.
        /// </summary>
        public IWorkflowExpression<bool> Condition
        {
            get => this.GetProperty(() => new JavaScriptExpression<bool>());
            set => this.SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(L["True"], L["False"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            var result = await ExpressionEvaluator.EvaluateAsync(Condition, workflowContext);
            return Outcomes(result ? "True" : "False");
        }
    }
}