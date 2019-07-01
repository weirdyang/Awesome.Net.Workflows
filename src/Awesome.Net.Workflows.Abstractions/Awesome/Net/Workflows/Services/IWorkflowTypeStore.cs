using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Services
{
    public interface IWorkflowTypeStore
    {
        Task<WorkflowType> GetAsync(Guid id);
        Task<WorkflowType> GetAsync(string uid);
        Task<IEnumerable<WorkflowType>> GetAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<WorkflowType>> ListAsync();
        Task<IEnumerable<WorkflowType>> GetByStartActivityAsync(string activityName);
        Task SaveAsync(WorkflowType workflowType);
        Task DeleteAsync(WorkflowType workflowType);
    }
}
