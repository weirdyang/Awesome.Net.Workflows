using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;

namespace Sample01
{
    public class ConsoleWriteLineTask : TaskActivity
    {
        public override LocalizedString Category => T["Console"];

        public WorkflowExpression<string> Text
        {
            get => GetExpressionProperty<string>();
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
            var text = await ExpressionEvaluator.EvaluateAsync(Text, workflowContext);
            Console.WriteLine(text);
            return Outcomes("Done");
        }

        public ConsoleWriteLineTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
