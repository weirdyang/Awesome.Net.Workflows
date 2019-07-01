using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Expressions.Syntaxs
{
    public interface ISyntaxEvaluator
    {
        string Syntax { get; }
        Task<T> EvaluateAsync<T>(string expression, WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken = default, params object[] @params);
    }
}