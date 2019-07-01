using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Logging;

namespace Awesome.Net.Workflows.Expressions
{
    public class WorkflowExpressionEvaluator : IWorkflowExpressionEvaluator
    {
        private readonly ILogger<WorkflowExpressionEvaluator> _logger;

        public IEnumerable<ISyntaxEvaluator> Evaluators { get; }

        public WorkflowExpressionEvaluator(IEnumerable<ISyntaxEvaluator> evaluators,
            ILogger<WorkflowExpressionEvaluator> logger)
        {
            Evaluators = evaluators;
            _logger = logger;
        }

        public async Task<T> EvaluateAsync<T>(IWorkflowExpression<T> expression, WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken = default, params object[] @params)
        {
            if(expression == null)
            {
                return default;
            }

            var evaluators = Evaluators.ToDictionary(x => x.Syntax);

            if(!evaluators.ContainsKey(expression.Syntax))
            {
                var ex = new Exception($"Cannot find any evaluator of {expression.Syntax}.");
                _logger.LogException(ex);

                throw ex;
            }

            var evaluator = evaluators[expression.Syntax];

            return await evaluator.EvaluateAsync<T>(expression.Expression, workflowExecutionContext, cancellationToken);
        }
    }
}