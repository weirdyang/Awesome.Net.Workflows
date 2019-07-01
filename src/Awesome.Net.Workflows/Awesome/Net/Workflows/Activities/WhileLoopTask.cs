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
    public class WhileLoopTask : TaskActivity, ITransientDependency
    {
        public override LocalizedString Category => L["ControlFlow"];

        /// <summary>
        /// An workflowExpression evaluating to true or false.
        /// </summary>
        public IWorkflowExpression<bool> Condition
        {
            get => this.GetProperty(() => new JavaScriptExpression<bool>());
            set => this.SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(L["Iterate"], L["Done"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            var loop = await ExpressionEvaluator.EvaluateAsync(Condition, workflowContext);
            return Outcomes(loop ? "Iterate" : "Done");
        }
    }
}