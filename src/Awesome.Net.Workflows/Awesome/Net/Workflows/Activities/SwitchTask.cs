using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;

namespace Awesome.Net.Workflows.Activities
{
    public class SwitchTask : TaskActivity
    {
        public override LocalizedString Category => T["Control Flow"];

        public SwitchTask(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Cases = new List<string>();
        }

        public WorkflowExpression<string> Expression
        {
            get => GetExpressionProperty<string>();
            set => SetProperty(value);
        }

        public IReadOnlyCollection<string> Cases
        {
            get => GetProperty<IReadOnlyCollection<string>>();
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Outcomes(Cases.Select(x => T[x]).ToArray());
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            var @case = await ExpressionEvaluator.EvaluateAsync(Expression, workflowContext);
            return Outcomes(@case);
        }
    }
}
