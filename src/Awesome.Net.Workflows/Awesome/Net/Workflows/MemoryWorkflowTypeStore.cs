using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Handlers;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Logging;

namespace Awesome.Net.Workflows
{
    public class MemoryWorkflowTypeStore : IWorkflowTypeStore
    {
        protected readonly ConcurrentDictionary<Guid, WorkflowType> Query = new ConcurrentDictionary<Guid, WorkflowType>();
        private readonly IEnumerable<IWorkflowTypePersistenceEventHandler> _handlers;
        public readonly ILogger<MemoryWorkflowTypeStore> _logger;

        public MemoryWorkflowTypeStore(
            IEnumerable<IWorkflowTypePersistenceEventHandler> handlers,
            ILogger<MemoryWorkflowTypeStore> logger)
        {
            _handlers = handlers;
            _logger = logger;
        }

        public Task<WorkflowType> GetAsync(Guid id)
        {
            var query = Query.GetOrDefault(id);
            return Task.FromResult(query);
        }

        public Task<WorkflowType> GetAsync(string uid)
        {
            var query = Query.Select(x => x.Value).FirstOrDefault(x => x.WorkflowTypeId == uid);
            return Task.FromResult(query);
        }

        public Task<IEnumerable<WorkflowType>> GetAsync(IEnumerable<Guid> ids)
        {
            var idList = ids.ToList();
            var query = Query.Select(x => x.Value).WhereIf(idList.Any(),
                x => idList.Contains(x.Id));

            return Task.FromResult(query);
        }

        public Task<IEnumerable<WorkflowType>> ListAsync()
        {
            var query = Query.Select(x => x.Value);
            return Task.FromResult(query);
        }

        public Task<IEnumerable<WorkflowType>> GetByStartActivityAsync(string activityName)
        {
            var query = Query.Select(x => x.Value).Where(x => x.Activities.Any(a => a.TypeName == activityName));
            query = query.Where(x => x.IsEnabled);

            return Task.FromResult(query);
        }

        public Task SaveAsync(WorkflowType workflowType)
        {
            if (Query.ContainsKey(workflowType.Id))
            {
                return UpdateAsync(workflowType);
            }
            else
            {
                return InsertAsync(workflowType);
            }
        }


        public async Task InsertAsync(WorkflowType workflowType)
        {
            Query[workflowType.Id] = workflowType;
            var context = new WorkflowTypeCreatedContext(workflowType);
            await _handlers.InvokeAsync(x => x.CreatedAsync(context), _logger);
        }

        public async Task DeleteAsync(WorkflowType workflowType)
        {
            Query.TryRemove(workflowType.Id, out _);

            var context = new WorkflowTypeDeletedContext(workflowType);
            await _handlers.InvokeAsync(x => x.DeletedAsync(context), _logger);
        }

        public async Task UpdateAsync(WorkflowType workflowType)
        {
            Query[workflowType.Id] = workflowType;

            var context = new WorkflowTypeUpdatedContext(workflowType);
            await _handlers.InvokeAsync(x => x.UpdatedAsync(context), _logger);
        }
    }
}
