using System.Collections.Generic;
using Awesome.Net.Scripting;

namespace Awesome.Net.Workflows.Contexts
{
    public class WorkflowExecutionScriptContext
    {
        public WorkflowExecutionScriptContext(WorkflowExecutionContext workflowContext)
        {
            WorkflowContext = workflowContext;
        }

        public WorkflowExecutionContext WorkflowContext { get; }

        public IList<IScriptMethodProvider> ScopedMethodProviders { get; } = new List<IScriptMethodProvider>();
    }
}
