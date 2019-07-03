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
    public class MemoryWorkflowStore : IWorkflowStore
    {
        protected readonly ConcurrentDictionary<Guid, Workflow> Query;
        private readonly IEnumerable<IWorkflowHandler> _handlers;
        private readonly ILogger<MemoryWorkflowStore> _logger;

        public MemoryWorkflowStore(IEnumerable<IWorkflowHandler> handlers, ILogger<MemoryWorkflowStore> logger)
        {
            _handlers = handlers;
            _logger = logger;
            Query = new ConcurrentDictionary<Guid, Workflow>();
        }

        public Task<int> CountAsync(string workflowTypeId = null)
        {
            var count = Query.Select(x => x.Value).WhereIf(!string.IsNullOrWhiteSpace(workflowTypeId),
                x => x.WorkflowTypeId == workflowTypeId).Count();

            return Task.FromResult(count);
        }

        public Task<IEnumerable<Workflow>> ListAsync(string workflowTypeId = null, int? skip = null, int? take = null)
        {
            var query = Query.Select(x => x.Value).WhereIf(!string.IsNullOrWhiteSpace(workflowTypeId),
                x => x.WorkflowTypeId == workflowTypeId);

            query = query.OrderByDescending(x => x.CreatedUtc);

            if (skip != null)
            {
                query = query.Skip(skip.Value);
            }

            if (take != null)
            {
                query = query.Take(take.Value);
            }

            return Task.FromResult(query);
        }

        public Task<IEnumerable<Workflow>> ListAsync(IEnumerable<string> workflowTypeIds)
        {
            var workflowTypeIdList = workflowTypeIds.ToList();
            var query = Query.Select(x => x.Value).WhereIf(workflowTypeIdList.Any(),
                x => workflowTypeIdList.Any(id => id == x.WorkflowTypeId));
            return Task.FromResult(query);
        }

        public Task<Workflow> GetAsync(Guid id)
        {
            var query = Query.GetOrDefault(id);
            return Task.FromResult(query);
        }

        public Task<Workflow> GetAsync(string workflowId)
        {
            var query = Query.Select(x => x.Value).FirstOrDefault(x => x.WorkflowId == workflowId);
            return Task.FromResult(query);
        }

        public Task<IEnumerable<Workflow>> GetAsync(IEnumerable<string> workflowIds)
        {
            var workflowIdList = workflowIds.ToList();
            var query = Query.Select(x => x.Value).WhereIf(workflowIdList.Any(),
                x => workflowIdList.Contains(x.WorkflowTypeId));

            return Task.FromResult(query);
        }

        public Task SaveAsync(Workflow workflow)
        {
            if (Query.ContainsKey(workflow.Id))
            {
                return UpdateAsync(workflow);
            }
            else
            {
                return InsertAsync(workflow);
            }
        }

        public Task<IEnumerable<Workflow>> GetAsync(IEnumerable<Guid> ids)
        {
            var idList = ids.ToList();
            var query = Query.Select(x => x.Value).WhereIf(idList.Any(),
                x => idList.Contains(x.Id));
            return Task.FromResult(query);
        }

        public Task<IEnumerable<Workflow>> ListAsync(string workflowTypeId, IEnumerable<string> blockingActivityIds)
        {
            var blockingActivityIdList = blockingActivityIds.ToList();
            var query = Query.Select(x => x.Value).Where(
                x => x.WorkflowTypeId == workflowTypeId);
            query = query.Where(x => x.BlockingActivities.Any(a => blockingActivityIdList.Contains(a.ActivityId)));
            return Task.FromResult(query);
        }

        public Task<IEnumerable<Workflow>> ListAsync(string workflowTypeId, string activityName,
            string correlationId = null)
        {
            var query = Query.Select(x => x.Value).Where(
                x => x.WorkflowTypeId == workflowTypeId);

            query = query.Where(x => x.BlockingActivities.Any(a => a.Name == activityName));
            query = query.WhereIf(!correlationId.IsNullOrEmpty(), x => x.CorrelationId == correlationId);

            return Task.FromResult(query);
        }

        public Task<IEnumerable<Workflow>> ListByActivityNameAsync(string activityName, string correlationId = null)
        {
            var query = Query.Select(x => x.Value).Where(x => x.BlockingActivities.Any(a => a.Name == activityName));
            query = query.WhereIf(!correlationId.IsNullOrEmpty(), x => x.CorrelationId == correlationId);

            return Task.FromResult(query);
        }

        public async Task InsertAsync(Workflow workflow)
        {
            Query[workflow.Id] = workflow;

            var context = new WorkflowCreatedContext(workflow);
            await _handlers.InvokeAsync(x => x.CreatedAsync(context), _logger);
        }

        public async Task DeleteAsync(Workflow workflow)
        {
            Query.TryRemove(workflow.Id, out _);

            var context = new WorkflowDeletedContext(workflow);
            await _handlers.InvokeAsync(x => x.DeletedAsync(context), _logger);
        }

        public async Task UpdateAsync(Workflow workflow)
        {
            Query[workflow.Id] = workflow;

            var context = new WorkflowUpdatedContext(workflow);
            await _handlers.InvokeAsync(x => x.UpdatedAsync(context), _logger);
        }
    }
}
