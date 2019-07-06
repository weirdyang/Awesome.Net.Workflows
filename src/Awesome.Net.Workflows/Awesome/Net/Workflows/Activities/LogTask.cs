using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Awesome.Net.Workflows.Activities
{
    public class LogTask : TaskActivity
    {
        public override LocalizedString Category => T["Primitives"];

        public LogLevel LogLevel
        {
            get => GetProperty(() => LogLevel.Information);
            set => SetProperty(value);
        }

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
            var logLevel = LogLevel;

            Logger.Log(logLevel, 0, text, null, (state, error) => state.ToString());
            return Outcomes("Done");
        }

        public LogTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
