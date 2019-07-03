using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Microsoft.Extensions.Logging;

namespace Awesome.Net.Workflows.Expressions
{
    public class WorkflowExpressionEvaluator : IWorkflowExpressionEvaluator
    {
        private readonly ILogger<WorkflowExpressionEvaluator> _logger;

        public IEnumerable<IExpressionEvaluator> Evaluators { get; }

        public WorkflowExpressionEvaluator(IEnumerable<IExpressionEvaluator> evaluators,
            ILogger<WorkflowExpressionEvaluator> logger)
        {
            Evaluators = evaluators;
            _logger = logger;
        }

        public async Task<T> EvaluateAsync<T>(IWorkflowExpression<T> expression,
            WorkflowExecutionContext workflowContext,
            IDictionary<string, object> arguments = null,
            CancellationToken cancellationToken = default)
        {
            if (expression == null)
            {
                return default;
            }

            var evaluators = Evaluators.ToDictionary(x => x.Syntax);

            if (!evaluators.ContainsKey(expression.Syntax))
            {
                var message = $"Cannot find any evaluator of {expression.Syntax}.";
                _logger.LogError(message);

                throw new Exception(message);
            }

            var evaluator = evaluators[expression.Syntax];

            return await evaluator.EvaluateAsync<T>(expression.Expression, workflowContext, arguments,
                cancellationToken);
        }
    }
}