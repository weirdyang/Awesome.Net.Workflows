using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;

namespace Awesome.Net.Workflows.Handlers
{
    public interface IWorkflowTypePersistenceEventHandler
    {
        Task CreatedAsync(WorkflowTypeCreatedContext context);
        Task UpdatedAsync(WorkflowTypeUpdatedContext context);
        Task DeletedAsync(WorkflowTypeDeletedContext context);
    }
}
