using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Awesome.Net.Workflows
{
    public class WorkflowManager : IWorkflowManager
    {
        public IServiceProvider ServiceProvide { get; }
        private readonly IActivityLibrary _activityLibrary;
        private readonly IWorkflowTypeStore _workflowTypeStore;
        private readonly IWorkflowStore _workflowStore;
        private readonly ILogger<WorkflowManager> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public WorkflowManager
        (
            IActivityLibrary activityLibrary,
            IWorkflowTypeStore workflowTypeRepository,
            IWorkflowStore workflowRepository,
            ILogger<WorkflowManager> logger,
            IServiceProvider serviceProvider,
            IDateTimeProvider dateTimeProvider)
        {
            _activityLibrary = activityLibrary;
            _workflowTypeStore = workflowTypeRepository;
            _workflowStore = workflowRepository;
            _logger = logger;
            ServiceProvide = serviceProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public Workflow NewWorkflow(WorkflowType workflowType, string correlationId = null)
        {
            var workflow = new Workflow
            {
                Id = Guid.NewGuid(),
                WorkflowTypeId = workflowType.WorkflowTypeId,
                Status = WorkflowStatus.Idle,
                State = JObject.FromObject(new WorkflowState
                {
                    ActivityStates = workflowType.Activities.Select(x => x)
                        .ToDictionary(x => x.ActivityId, x => x.Properties)
                }),
                CorrelationId = correlationId,
                CreatedUtc = _dateTimeProvider.Now,
                WorkflowId = RandomHelper.Generate26UniqueId()
            };

            return workflow;
        }

        public async Task<WorkflowExecutionContext> CreateWorkflowExecutionContextAsync(WorkflowType workflowType,
            Workflow workflow, IDictionary<string, object> input = null)
        {
            var state = workflow.State.ToObject<WorkflowState>();
            var activityQuery = await Task.WhenAll(workflowType.Activities.Select(async x =>
            {
                var activityState = state.ActivityStates.ContainsKey(x.ActivityId)
                    ? state.ActivityStates[x.ActivityId]
                    : new JObject();
                return await CreateActivityExecutionContextAsync(x, activityState);
            }));
            var mergedInput = state.Input.Merge(input ?? new Dictionary<string, object>());
            var properties = state.Properties;
            var output = state.Output;
            var lastResult = state.LastResult;
            var executedActivities = state.ExecutedActivities;

            return new WorkflowExecutionContext(workflowType,
                workflow,
                mergedInput,
                output,
                properties,
                executedActivities,
                lastResult,
                activityQuery);
        }

        public Task<ActivityExecutionContext> CreateActivityExecutionContextAsync(ActivityRecord activityRecord,
            JObject properties)
        {
            var activity = _activityLibrary.InstantiateActivity<IActivity>(activityRecord.TypeName, properties);

            if (activity == null)
            {
                _logger.LogWarning(
                    "Requested activity '{ActivityName}' does not exist in the library. This could indicate a changed typeName or a missing feature. Replacing it with MissingActivity.",
                    activityRecord.TypeName);
                activity = new MissingActivity(ServiceProvide, activityRecord);
            }

            var context = new ActivityExecutionContext { ActivityRecord = activityRecord, Activity = activity };

            return Task.FromResult(context);
        }

        public async Task TriggerEventAsync(string typeName, IDictionary<string, object> input = null,
            string correlationId = null)
        {
            var activity = _activityLibrary.GetActivityByName(typeName);

            if (activity == null)
            {
                _logger.LogError("Activity '{ActivityName}' was not found", typeName);
                return;
            }

            // Look for workflow types with a corresponding starting activity.
            var workflowTypesToStart = await _workflowTypeStore.GetByStartActivityAsync(typeName);

            // And any workflow halted on this kind of activity for the specified target.
            var haltedWorkflows = await _workflowStore.ListByActivityNameAsync(typeName, correlationId);

            // If no workflow matches the event, do nothing.
            if (!workflowTypesToStart.Any() && !haltedWorkflows.Any())
            {
                return;
            }

            // Start new workflows.
            foreach (var workflowType in workflowTypesToStart)
            {
                // If this is a singleton workflow and there's already an instance, then skip.
                if (workflowType.IsSingleton &&
                    haltedWorkflows.Any(x => x.WorkflowTypeId == workflowType.WorkflowTypeId))
                {
                    continue;
                }

                var startActivity = workflowType.Activities.FirstOrDefault(x =>
                    x.IsStart && string.Equals(x.TypeName, typeName, StringComparison.OrdinalIgnoreCase));

                if (startActivity != null)
                {
                    await StartWorkflowAsync(workflowType, startActivity, input, correlationId);
                }
            }

            // Resume halted workflows.
            foreach (var workflow in haltedWorkflows)
            {
                var blockingActivities = workflow.BlockingActivities.Where(x => x.Name == typeName).ToList();

                foreach (var blockingActivity in blockingActivities)
                {
                    await ResumeWorkflowAsync(workflow, blockingActivity, input);
                }
            }
        }

        public async Task<WorkflowExecutionContext> ResumeWorkflowAsync(Workflow workflow,
            BlockingActivity awaitingActivity, IDictionary<string, object> input = null)
        {
            var workflowType = await _workflowTypeStore.GetAsync(workflow.WorkflowTypeId);
            var activityRecord =
                workflowType.Activities.SingleOrDefault(x => x.ActivityId == awaitingActivity.ActivityId);
            var workflowContext = await CreateWorkflowExecutionContextAsync(workflowType, workflow, input);

            workflowContext.Status = WorkflowStatus.Resuming;

            // Signal every activity that the workflow is about to be resumed.
            var cancellationToken = new CancellationToken();

            await InvokeActivitiesAsync(workflowContext, x => x.Activity.OnInputReceivedAsync(workflowContext, input));
            await InvokeActivitiesAsync(workflowContext,
                x => x.Activity.OnWorkflowResumingAsync(workflowContext, cancellationToken));

            if (cancellationToken.IsCancellationRequested)
            {
                // Workflow is aborted.
                workflowContext.Status = WorkflowStatus.Aborted;
            }
            else
            {
                // Check if the current activity can execute.
                if (activityRecord != null)
                {
                    var activityContext = workflowContext.GetActivity(activityRecord.ActivityId);
                    if (await activityContext.Activity.CanExecuteAsync(workflowContext, activityContext))
                    {
                        // Signal every activity that the workflow is resumed.
                        await InvokeActivitiesAsync(workflowContext,
                            x => x.Activity.OnWorkflowResumedAsync(workflowContext));

                        // Remove the blocking activity.
                        workflowContext.Workflow.BlockingActivities.Remove(awaitingActivity);

                        // Resume the workflow at the specified blocking activity.
                        await ExecuteWorkflowAsync(workflowContext, activityRecord);
                    }
                    else
                    {
                        workflowContext.Status = WorkflowStatus.Halted;
                        return workflowContext;
                    }
                }
            }

            if (workflowContext.Status == WorkflowStatus.Finished && workflowType.DeleteFinishedWorkflows)
            {
                await _workflowStore.DeleteAsync(workflowContext.Workflow);
            }
            else
            {
                await PersistAsync(workflowContext);
            }

            return workflowContext;
        }

        public async Task<WorkflowExecutionContext> StartWorkflowAsync(WorkflowType workflowType,
            ActivityRecord startActivity = null, IDictionary<string, object> input = null, string correlationId = null)
        {
            if (startActivity == null)
            {
                startActivity = workflowType.Activities.FirstOrDefault(x => x.IsStart);

                if (startActivity == null)
                {
                    throw new InvalidOperationException(
                        $"Workflow with ID {workflowType.Id} does not have a start activity.");
                }
            }

            // Create a new workflow instance.
            var workflow = NewWorkflow(workflowType, correlationId);

            // Create a workflow context.
            var workflowContext = await CreateWorkflowExecutionContextAsync(workflowType, workflow, input);
            workflowContext.Status = WorkflowStatus.Starting;

            // Signal every activity about available input.
            await InvokeActivitiesAsync(workflowContext, x => x.Activity.OnInputReceivedAsync(workflowContext, input));

            // Signal every activity that the workflow is about to start.
            var cancellationToken = new CancellationToken();
            await InvokeActivitiesAsync(workflowContext,
                x => x.Activity.OnWorkflowStartingAsync(workflowContext, cancellationToken));

            if (cancellationToken.IsCancellationRequested)
            {
                // Workflow is aborted.
                workflowContext.Status = WorkflowStatus.Aborted;
                return workflowContext;
            }
            else
            {
                // Check if the current activity can execute.
                var activityContext = workflowContext.GetActivity(startActivity.ActivityId);
                if (await activityContext.Activity.CanExecuteAsync(workflowContext, activityContext))
                {
                    // Signal every activity that the workflow has started.
                    await InvokeActivitiesAsync(workflowContext,
                        x => x.Activity.OnWorkflowStartedAsync(workflowContext));

                    // Execute the activity.
                    await ExecuteWorkflowAsync(workflowContext, startActivity);
                }
                else
                {
                    workflowContext.Status = WorkflowStatus.Idle;
                    return workflowContext;
                }
            }

            if (workflowContext.Status != WorkflowStatus.Finished || !workflowType.DeleteFinishedWorkflows)
            {
                // Serialize state.
                await PersistAsync(workflowContext);
            }

            return workflowContext;
        }

        public async Task<IEnumerable<ActivityRecord>> ExecuteWorkflowAsync(WorkflowExecutionContext workflowContext,
            ActivityRecord activity)
        {
            var workflowType = workflowContext.WorkflowType;
            var scheduled = new Stack<ActivityRecord>();
            var blocking = new List<ActivityRecord>();
            var isResuming = workflowContext.Status == WorkflowStatus.Resuming;
            var isFirstPass = true;

            workflowContext.Status = WorkflowStatus.Executing;
            scheduled.Push(activity);

            while (scheduled.Count > 0)
            {
                activity = scheduled.Pop();

                var activityContext = workflowContext.GetActivity(activity.ActivityId);

                // Signal every activity that the activity is about to be executed.
                var cancellationToken = new CancellationToken();
                await InvokeActivitiesAsync(workflowContext,
                    x => x.Activity.OnActivityExecutingAsync(workflowContext, activityContext, cancellationToken));

                if (cancellationToken.IsCancellationRequested)
                {
                    // Activity is aborted.
                    workflowContext.Status = WorkflowStatus.Aborted;
                    break;
                }

                IList<string> outcomes;

                try
                {
                    ActivityExecutionResult result;

                    if (!isResuming)
                    {
                        // Execute the current activity.
                        result = await activityContext.Activity.ExecuteAsync(workflowContext, activityContext);
                    }
                    else
                    {
                        // Resume the current activity.
                        result = await activityContext.Activity.ResumeAsync(workflowContext, activityContext);
                        isResuming = false;
                    }

                    if (result.IsHalted)
                    {
                        if (isFirstPass)
                        {
                            // Resume immediately when this is the first pass.
                            result = await activityContext.Activity.ResumeAsync(workflowContext, activityContext);
                            outcomes = result.Outcomes;

                            if (result.IsHalted)
                            {
                                // Block on this activity.
                                blocking.Add(activity);
                            }
                        }
                        else
                        {
                            // Block on this activity.
                            blocking.Add(activity);

                            continue;
                        }
                    }
                    else
                    {
                        outcomes = result.Outcomes;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "An unhandled error occurred while executing an activity. Workflow ID: '{WorkflowTypeId}'. Activity: '{ActivityId}', '{ActivityName}'. Putting the workflow in the faulted state.",
                        workflowType.Id, activityContext.ActivityRecord.ActivityId,
                        activityContext.ActivityRecord.TypeName);
                    workflowContext.Fault(ex, activityContext);
                    return blocking.Distinct();
                }

                // Signal every activity that the activity is executed.
                await InvokeActivitiesAsync(workflowContext,
                    x => x.Activity.OnActivityExecutedAsync(workflowContext, activityContext));

                foreach (var outcome in outcomes)
                {
                    // Look for next activity in the graph.
                    var transition = workflowType.Transitions.FirstOrDefault(x =>
                        x.SourceActivityId == activity.ActivityId && x.SourceOutcomeName == outcome);

                    if (transition != null)
                    {
                        var destinationActivity =
                            workflowContext.WorkflowType.Activities.SingleOrDefault(x =>
                                x.ActivityId == transition.DestinationActivityId);
                        scheduled.Push(destinationActivity);
                    }
                }

                isFirstPass = false;
            }

            // Apply Distinct() as two paths could block on the same activity.
            var blockingActivities = blocking.Distinct().ToList();

            workflowContext.Status = blockingActivities.Any() || workflowContext.Workflow.BlockingActivities.Any()
                ? WorkflowStatus.Halted
                : WorkflowStatus.Finished;

            foreach (var blockingActivity in blockingActivities)
            {
                // Workflows containing event activities could end up being blocked on the same activity.
                if (workflowContext.Workflow.BlockingActivities.All(x => x.ActivityId != blockingActivity.ActivityId))
                {
                    workflowContext.Workflow.BlockingActivities.Add(BlockingActivity.FromActivity(blockingActivity));
                }
            }

            return blockingActivities;
        }

        private async Task PersistAsync(WorkflowExecutionContext workflowContext)
        {
            var state = workflowContext.Workflow.State.ToObject<WorkflowState>();

            state.Input = workflowContext.Input;
            state.Output = workflowContext.Output;
            state.Properties = workflowContext.Properties;
            state.LastResult = workflowContext.LastResult;
            state.ExecutedActivities = workflowContext.ExecutedActivities.ToList();
            state.ActivityStates =
                workflowContext.Activities.ToDictionary(x => x.Key, x => x.Value.Activity.Properties);

            workflowContext.Workflow.State = JObject.FromObject(state);
            await _workflowStore.SaveAsync(workflowContext.Workflow);
        }

        /// <summary>
        /// Executes a specific action on all the activities of a workflow.
        /// </summary>
        private Task InvokeActivitiesAsync(WorkflowExecutionContext workflowContext,
            Func<ActivityExecutionContext, Task> action)
        {
            return workflowContext.Activities.Values.InvokeAsync(action, _logger);
        }
    }
}
