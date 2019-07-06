using System;
using System.Collections.Generic;
using Awesome.Net.Scripting;
using Awesome.Net.Workflows.Contexts;

namespace Awesome.Net.Workflows.Scripting
{
    public class WorkflowScriptMethodsProvider : IScriptMethodProvider
    {
        private readonly ScriptMethod _workflowMethod;
        private readonly ScriptMethod _workflowIdMethod;
        private readonly ScriptMethod _inputMethod;
        private readonly ScriptMethod _outputMethod;
        private readonly ScriptMethod _propertyMethod;
        private readonly ScriptMethod _setPropertyMethod;
        private readonly ScriptMethod _resultMethod;
        private readonly ScriptMethod _correlationIdMethod;

        public WorkflowScriptMethodsProvider(WorkflowExecutionContext workflowContext)
        {
            _workflowMethod = new ScriptMethod
            {
                Name = "workflow", Method = serviceProvider => (Func<object>) (() => workflowContext)
            };

            _workflowIdMethod = new ScriptMethod
            {
                Name = "workflowId",
                Method = serviceProvider => (Func<string>) (() => workflowContext.Workflow.WorkflowId)
            };

            _inputMethod = new ScriptMethod
            {
                Name = "input",
                Method = serviceProvider => (Func<string, object>) (name => workflowContext.Input[name])
            };

            _outputMethod = new ScriptMethod
            {
                Name = "output",
                Method = serviceProvider =>
                    (Action<string, object>) ((name, value) => workflowContext.Output[name] = value)
            };

            _propertyMethod = new ScriptMethod
            {
                Name = "property",
                Method = serviceProvider => (Func<string, object>) ((name) => workflowContext.Properties[name])
            };

            _setPropertyMethod = new ScriptMethod
            {
                Name = "setProperty",
                Method = serviceProvider =>
                    (Action<string, object>) ((name, value) => workflowContext.Properties[name] = value)
            };

            _resultMethod = new ScriptMethod
            {
                Name = "lastResult", Method = serviceProvider => (Func<object>) (() => workflowContext.LastResult)
            };

            _correlationIdMethod = new ScriptMethod
            {
                Name = "correlationId",
                Method = serviceProvider => (Func<string>) (() => workflowContext.Workflow.CorrelationId)
            };
        }

        public IEnumerable<ScriptMethod> GetMethods()
        {
            return new[]
            {
                _workflowMethod, _workflowIdMethod, _inputMethod, _outputMethod, _propertyMethod, _resultMethod,
                _correlationIdMethod, _setPropertyMethod
            };
        }
    }
}
