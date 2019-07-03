using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions.Syntaxs;

namespace Awesome.Net.Workflows.Expressions
{
    public interface IWorkflowExpressionEvaluator
    {
        IEnumerable<IExpressionEvaluator> Evaluators { get; }

        Task<T> EvaluateAsync<T>(IWorkflowExpression<T> workflowExpression,
            WorkflowExecutionContext workflowContext, IDictionary<string, object> arguments = null,
            CancellationToken cancellationToken = default);
    }
}
