using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awesome.Net.Scripting;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Awesome.Net.Workflows.Scripting;
using Microsoft.Extensions.Localization;

namespace Awesome.Net.Workflows.Activities
{
    public class ScriptTask : TaskActivity
    {
        public override LocalizedString Category => T["Control Flow"];

        public IList<string> AvailableOutcomes
        {
            get => GetProperty(() => new List<string> {"Done"});
            set => SetProperty(value);
        }

        /// <summary>
        /// The script can call any available functions, including setOutcome().
        /// </summary>
        public IWorkflowExpression<object> Script
        {
            get => GetProperty(() => new JavaScriptExpression<object>("setOutcome('Done');"));
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Outcomes(AvailableOutcomes.Select(x => T[x]).ToArray());
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            var outcomes = new List<string>();

            var scopedMethodProviders = new List<IGlobalMethodProvider> {new OutcomeMethodProvider(outcomes)};

            await ExpressionEvaluator.EvaluateAsync(Script, workflowContext, scopedMethodProviders.AsDictionary());
            return Outcomes(outcomes);
        }

        public ScriptTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}