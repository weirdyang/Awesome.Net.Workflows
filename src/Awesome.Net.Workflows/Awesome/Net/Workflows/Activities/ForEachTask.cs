using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awesome.Net.Data;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;

namespace Awesome.Net.Workflows.Activities
{
    public class ForEachTask : TaskActivity, ITransientDependency
    {
        public override LocalizedString Category => L["ControlFlow"];

        /// <summary>
        /// An workflowExpression evaluating to an enumerable object to iterate over.
        /// </summary>
        public IWorkflowExpression<IEnumerable<object>> Enumerable
        {
            get => this.GetProperty(() => new JavaScriptExpression<IEnumerable<object>>());
            set => this.SetProperty(value);
        }

        /// <summary>
        /// The current iteration value.
        /// </summary>
        public string LoopVariableName
        {
            get => this.GetProperty(() => "x");
            set => this.SetProperty(value);
        }

        /// <summary>
        /// The current iteration value.
        /// </summary>
        public object Current
        {
            get => this.GetProperty<object>();
            set => this.SetProperty(value);
        }

        /// <summary>
        /// The current number of iterations executed.
        /// </summary>
        public int Index
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
            var items = (await ExpressionEvaluator.EvaluateAsync(Enumerable, workflowContext)).ToList();
            var count = items.Count;

            if(Index < count)
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
    }
}
