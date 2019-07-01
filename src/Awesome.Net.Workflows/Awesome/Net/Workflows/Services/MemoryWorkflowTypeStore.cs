using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Events;
using Awesome.Net.Workflows.Models;
using Volo.Abp.EventBus.Distributed;

namespace Awesome.Net.Workflows.Services
{
    public class MemoryWorkflowTypeStore : IWorkflowTypeStore
    {
        private readonly IDistributedEventBus _eventBus;
        protected readonly ConcurrentDictionary<Guid, WorkflowType> Query;

        public MemoryWorkflowTypeStore(IDistributedEventBus eventBus, ConcurrentDictionary<Guid, WorkflowType> query)
        {
            _eventBus = eventBus;
            Query = query;
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
                x => x.Id.IsIn(idList));
            return Task.FromResult(query);
        }

        public Task<IEnumerable<WorkflowType>> ListAsync()
        {
            var query = Query.Select(x => x.Value);
            return Task.FromResult(query);
        }

        public Task<IEnumerable<WorkflowType>> GetByStartActivityAsync(string activityName)
        {
            var query = Query.Select(x => x.Value).Where(x => x.Activities.Any(a => a.Name == activityName));
            query = query.Where(x => x.IsEnabled);

            return Task.FromResult(query);
        }

        public Task SaveAsync(WorkflowType workflowType)
        {
            if(Query.ContainsKey(workflowType.Id))
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
            await _eventBus.PublishAsync(new WorkflowTypeCreatedEventData(workflowType));
        }

        public async Task DeleteAsync(WorkflowType workflowType)
        {
            Query.TryRemove(workflowType.Id, out _);

            await _eventBus.PublishAsync(new WorkflowTypeDeletedEventData(workflowType));
        }

        public async Task UpdateAsync(WorkflowType workflowType)
        {
            Query[workflowType.Id] = workflowType;

            await _eventBus.PublishAsync(new WorkflowTypeUpdatedEventData(workflowType));
        }
    }
}
