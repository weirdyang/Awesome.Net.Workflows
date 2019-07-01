using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Scripting;
using Awesome.Net.Workflows.Events;
using Awesome.Net.Workflows.Expressions.Syntaxs;
using Awesome.Net.Workflows.Models;
using Volo.Abp.EventBus.Local;

namespace Awesome.Net.Workflows.Expressions
{
    public class JavaScriptEvaluator : ISyntaxEvaluator
    {
        private readonly ILocalEventBus _localEventBus;
        private readonly IScriptingManager _scriptingManager;
        public string Syntax => SyntaxNameAttribute.GetSyntax(typeof(JavaScriptExpression));

        public JavaScriptEvaluator(
            IScriptingManager scriptingManager,
            ILocalEventBus localEventBus)
        {
            _scriptingManager = scriptingManager;
            _localEventBus = localEventBus;
        }

        public async Task<T> EvaluateAsync<T>(string expression, WorkflowExecutionContext workflowExecutionContext, CancellationToken cancellationToken = default, params object[] @params)
        {
            if(string.IsNullOrWhiteSpace(expression))
            {
                return await Task.FromResult(default(T));
            }

            var directive = $"js:{expression}";

            var eventData = new EvaluatingScriptEventData(workflowExecutionContext);

            await _localEventBus.PublishAsync(eventData);

            var methodProviders = eventData.ScopedMethodProviders;

            var scopedMethodProviders = @params?.As<IEnumerable<IGlobalMethodProvider>>();
            if(scopedMethodProviders != null)
            {
                methodProviders = methodProviders.Concat(scopedMethodProviders).ToList();
            }

            return (T)_scriptingManager.Evaluate(directive, null, null, methodProviders);
        }
    }
}
