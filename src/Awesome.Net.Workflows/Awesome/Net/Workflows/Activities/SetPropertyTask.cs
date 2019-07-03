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
    public class SetPropertyTask : TaskActivity
    {
        public override LocalizedString Category => T["Primitives"];

        public string PropertyName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public IWorkflowExpression<object> Value
        {
            get => GetProperty(() => new JavaScriptExpression<object>());
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Outcomes(T["Done"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            var value = await ExpressionEvaluator.EvaluateAsync(Value, workflowContext);
            workflowContext.Properties[PropertyName] = value;

            return Outcomes("Done");
        }

        public SetPropertyTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
