using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;

namespace Awesome.Net.Workflows.Expressions.Syntaxs
{
    public interface IExpressionEvaluator
    {
        string Syntax { get; }

        Task<T> EvaluateAsync<T>(string expression,
            WorkflowExecutionContext workflowContext,
            IDictionary<string, object> arguments = null,
            CancellationToken cancellationToken = default);
    }
}