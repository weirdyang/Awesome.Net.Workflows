using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;

namespace Awesome.Net.Workflows.Handlers
{
    public interface IWorkflowHandler
    {
        Task CreatedAsync(WorkflowCreatedContext context);
        Task UpdatedAsync(WorkflowUpdatedContext context);
        Task DeletedAsync(WorkflowDeletedContext context);
    }
}
