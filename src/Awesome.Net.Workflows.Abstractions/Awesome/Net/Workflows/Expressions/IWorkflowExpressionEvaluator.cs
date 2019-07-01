using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Expressions
{
    public interface IWorkflowExpressionEvaluator
    {
        IEnumerable<ISyntaxEvaluator> Evaluators { get; }

        Task<T> EvaluateAsync<T>(IWorkflowExpression<T> workflowExpression, WorkflowExecutionContext workflowContext, CancellationToken cancellationToken = default, params object[] @params);
    }
}
