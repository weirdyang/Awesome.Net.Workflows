using System.Collections.Generic;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace Awesome.Net.Workflows.Activities
{
    /// <summary>
    /// A replacement for when a referenced activity could not be found. This is a system-level activity that is not registered with WorkflowOptions.
    /// </summary>
    public class MissingActivity : Activity, ITransientDependency
    {
        public MissingActivity( ActivityRecord missingActivityRecord)
        {
            MissingActivityRecord = missingActivityRecord;
        }

        public override LocalizedString Category => L["Exceptions"];

        public ActivityRecord MissingActivityRecord { get; }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            yield break;
        }

        public override ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext, ActivityContext activityContext)
        {
            Logger.LogWarning("Activity '{ActivityName}' is no longer available. This can happen if the feature providing the activity is no longer enabled. Either enable the feature, or remove this activity from workflow definition with ID {WorkflowTypeId}", MissingActivityRecord.Name, workflowContext.WorkflowType.Id);
            return Noop();
        }
    }
}