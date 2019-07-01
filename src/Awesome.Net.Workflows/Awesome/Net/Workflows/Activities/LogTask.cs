using System.Collections.Generic;
using System.Threading.Tasks;
using Awesome.Net.Data;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Awesome.Net.Workflows.Activities
{
    public class LogTask : TaskActivity, ITransientDependency
    {
        public override LocalizedString Category => L["Primitives"];

        public LogLevel LogLevel
        {
            get => this.GetProperty(() => LogLevel.Information);
            set => this.SetProperty(value);
        }

        public IWorkflowExpression<string> Text
        {
            get => this.GetProperty(() => new LiquidExpression());
            set => this.SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            return Outcomes(L["Done"]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            var text = await ExpressionEvaluator.EvaluateAsync(Text, workflowContext);
            var logLevel = LogLevel;

            Logger.Log(logLevel, 0, text, null, (state, error) => state.ToString());
            return Outcomes("Done");
        }
    }
}