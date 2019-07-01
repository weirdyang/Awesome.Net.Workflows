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
    public class ForLoopTask : TaskActivity, ITransientDependency
    {
        public override LocalizedString Category => L["ControlFlow"];

        /// <summary>
        /// An workflowExpression evaluating to the start value.
        /// </summary>
        public IWorkflowExpression<double> From
        {
            get => this.GetProperty(() => new JavaScriptExpression<double>("0"));
            set => this.SetProperty(value);
        }

        /// <summary>
        /// An workflowExpression evaluating to the end value.
        /// </summary>
        public IWorkflowExpression<double> To
        {
            get => this.GetProperty(() => new JavaScriptExpression<double>("10"));
            set => this.SetProperty(value);
        }

        /// <summary>
        /// An workflowExpression evaluating to the end value.
        /// </summary>
        public IWorkflowExpression<double> Step
        {
            get => this.GetProperty(() => new JavaScriptExpression<double>("1"));
            set => this.SetProperty(value);
        }

        /// <summary>
        /// The property name to store the current iteration number in.
        /// </summary>
        public string LoopVariableName
        {
            get => this.GetProperty(() => "x");
            set => this.SetProperty(value);
        }

        /// <summary>
        /// The current index of the iteration.
        /// </summary>
        public double Index
        {
            get => this.GetProperty(() => 0);
            set => this.SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(L["Iterate"], L["Done"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            if(!double.TryParse(From.Expression, out var from))
            {
                from = await ExpressionEvaluator.EvaluateAsync(From, workflowContext);
            }

            if(!double.TryParse(To.Expression, out var to))
            {
                to = await ExpressionEvaluator.EvaluateAsync(To, workflowContext);
            }

            if(!double.TryParse(Step.Expression, out var step))
            {
                step = await ExpressionEvaluator.EvaluateAsync(Step, workflowContext);
            }

            if(Index < from)
            {
                Index = from;
            }

            if(Index < to)
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
    }
}