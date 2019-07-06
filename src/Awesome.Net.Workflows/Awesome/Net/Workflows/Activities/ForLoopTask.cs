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
    public class ForLoopTask : TaskActivity
    {
        public override LocalizedString Category => T["Control Flow"];

        /// <summary>
        /// An workflowExpression evaluating to the start value.
        /// </summary>
        public WorkflowExpression<double> From
        {
            get => GetExpressionProperty(new JavaScriptExpr<double>("0"));
            set => SetProperty(value);
        }

        /// <summary>
        /// An workflowExpression evaluating to the end value.
        /// </summary>
        public WorkflowExpression<double> To
        {
            get => GetExpressionProperty(new JavaScriptExpr<double>("10"));
            set => SetProperty(value);
        }

        /// <summary>
        /// An workflowExpression evaluating to the end value.
        /// </summary>
        public WorkflowExpression<double> Step
        {
            get => GetExpressionProperty(new JavaScriptExpr<double>("1"));
            set => SetProperty(value);
        }

        /// <summary>
        /// The property name to store the current iteration number in.
        /// </summary>
        public string LoopVariableName
        {
            get => GetProperty(() => "x");
            set => SetProperty(value);
        }

        /// <summary>
        /// The current index of the iteration.
        /// </summary>
        public double Index
        {
            get => GetProperty(() => 0);
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
            if (!double.TryParse(From.Expression, out var from))
            {
                from = await ExpressionEvaluator.EvaluateAsync(From, workflowContext);
            }

            if (!double.TryParse(To.Expression, out var to))
            {
                to = await ExpressionEvaluator.EvaluateAsync(To, workflowContext);
            }

            if (!double.TryParse(Step.Expression, out var step))
            {
                step = await ExpressionEvaluator.EvaluateAsync(Step, workflowContext);
            }

            if (Index < from)
            {
                Index = from;
            }

            if (Index < to)
            {
                workflowContext.LastResult = Index;
                workflowContext.Properties[LoopVariableName] = Index;
                Index += step;
                return Outcomes("Iterate");
            }
            else
            {
                Index = from;
                return Outcomes("Done");
            }
        }

        public ForLoopTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
