using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;

namespace Awesome.Net.Workflows.Activities
{
    public class IfElseTask : TaskActivity
    {
        public override LocalizedString Category => T["Control Flow"];

        /// <summary>
        /// A script evaluating to either true or false.
        /// </summary>
        public IWorkflowExpression<bool> Condition
        {
            get => GetProperty(() => new JavaScriptExpression<bool>());
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Outcomes(T["True"], T["False"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            var result = await ExpressionEvaluator.EvaluateAsync(Condition, workflowContext);
            return Outcomes(result ? "True" : "False");
        }

        public IfElseTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}