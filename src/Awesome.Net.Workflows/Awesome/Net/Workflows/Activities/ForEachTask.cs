using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;

namespace Awesome.Net.Workflows.Activities
{
    public class ForEachTask : TaskActivity
    {
        public override LocalizedString Category => T["Control Flow"];

        /// <summary>
        /// An workflowExpression evaluating to an enumerable object to iterate over.
        /// </summary>
        public IWorkflowExpression<IEnumerable<object>> Enumerable
        {
            get => GetProperty(() => new JavaScriptExpression<IEnumerable<object>>());
            set => SetProperty(value);
        }

        /// <summary>
        /// The current iteration value.
        /// </summary>
        public string LoopVariableName
        {
            get => GetProperty(() => "x");
            set => SetProperty(value);
        }

        /// <summary>
        /// The current iteration value.
        /// </summary>
        public object Current
        {
            get => GetProperty<object>();
            set => SetProperty(value);
        }

        /// <summary>
        /// The current number of iterations executed.
        /// </summary>
        public int Index
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
            var items = (await ExpressionEvaluator.EvaluateAsync(Enumerable, workflowContext)).ToList();
            var count = items.Count;

            if (Index < count)
            {
                var current = Current = items[Index];

                // TODO: Implement nested scopes. See https://github.com/OrchardCMS/OrchardCore/projects/4#card-6992776
                workflowContext.Properties[LoopVariableName] = current;
                workflowContext.LastResult = current;
                Index++;
                return Outcomes("Iterate");
            }
            else
            {
                Index = 0;
                return Outcomes("Done");
            }
        }

        public ForEachTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}