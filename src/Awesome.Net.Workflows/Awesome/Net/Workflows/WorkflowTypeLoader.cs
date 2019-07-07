using System;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows
{
    public class WorkflowTypeLoader : IWorkflowTypeLoader
    {
        public WorkflowType Load(string source, Func<string, WorkflowType> deserializer)
        {
            return deserializer.Invoke(source);
        }
    }
}
