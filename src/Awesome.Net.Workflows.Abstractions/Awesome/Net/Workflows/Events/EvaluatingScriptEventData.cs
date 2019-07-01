using System.Collections.Generic;
using Awesome.Net.Scripting;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Events
{
    public class EvaluatingScriptEventData
    {
        public EvaluatingScriptEventData(WorkflowExecutionContext workflowExecutionContext)
        {
            WorkflowExecutionContext = workflowExecutionContext;
        }

        public WorkflowExecutionContext WorkflowExecutionContext { get; }
        public IList<IGlobalMethodProvider> ScopedMethodProviders { get; } = new List<IGlobalMethodProvider>();
    }
}