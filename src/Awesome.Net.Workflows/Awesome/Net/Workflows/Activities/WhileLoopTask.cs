using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;

namespace Awesome.Net.Workflows.Activities
{
    public class WhileLoopTask : TaskActivity
    {
        public override LocalizedString Category => T["Control Flow"];

        /// <summary>
        /// An workflowExpression evaluating to true or false.
        /// </summary>
        public WorkflowExpression<bool> Condition
        {
            get => GetExpressionProperty<bool>();
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Outcomes(T["Iterate"], T["Done"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            var loop = await ExpressionEvaluator.EvaluateAsync(Condition, workflowContext);
            return Outcomes(loop ? "Iterate" : "Done");
        }

        public WhileLoopTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
