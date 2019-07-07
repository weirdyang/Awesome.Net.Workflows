using System;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows
{
    public interface IWorkflowTypeLoader
    {
        WorkflowType Load(string source, Func<string, WorkflowType> deserializer);
    }
}
