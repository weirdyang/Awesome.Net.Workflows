using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Scripting;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Handlers;
using Microsoft.Extensions.Logging;

namespace Awesome.Net.Workflows.Expressions
{
    public class JavaScriptExpressionEvaluator : IExpressionEvaluator
    {
        private readonly IEnumerable<IWorkflowExecutionContextHandler> _workflowContextHandlers;
        private readonly ILogger<JavaScriptExpressionEvaluator> _logger;

        private readonly IScriptingManager _scriptingManager;
        public string Syntax => SyntaxNameAttribute.GetSyntax(typeof(JavaScriptExpression));

        public JavaScriptExpressionEvaluator(
            IScriptingManager scriptingManager,
            IEnumerable<IWorkflowExecutionContextHandler> workflowContextHandlers,
            ILogger<JavaScriptExpressionEvaluator> logger)
        {
            _scriptingManager = scriptingManager;
            _workflowContextHandlers = workflowContextHandlers;
            _logger = logger;
        }

        public async Task<T> EvaluateAsync<T>(string expression,
            WorkflowExecutionContext workflowContext,
            IDictionary<string, object> arguments = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return default;
            }

            var directive = $"js:{expression}";

            var expressionContext = new WorkflowExecutionScriptContext(workflowContext);

            await _workflowContextHandlers.InvokeAsync(x => x.EvaluatingScriptAsync(expressionContext), _logger);

            var methodProviders = expressionContext.ScopedMethodProviders;

            var scopedMethodProviders = arguments.As<List<IScriptMethodProvider>>();
            if (scopedMethodProviders != null)
            {
                methodProviders = methodProviders.Concat(scopedMethodProviders).ToList();
            }

            return (T) _scriptingManager.Evaluate(directive, null, null, methodProviders);
        }
    }
}
