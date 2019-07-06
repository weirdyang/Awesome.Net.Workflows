using System;
using System.Collections.Generic;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Awesome.Net.Workflows.Activities
{
    /// <summary>
    /// A replacement for when a referenced activity could not be found. This is a system-level activity that is not registered with WorkflowOptions.
    /// </summary>
    public class MissingActivity : Activity
    {
        public override LocalizedString Category => T["Exceptions"];

        public ActivityRecord MissingActivityRecord { get; }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            yield break;
        }

        public override ActivityExecutionResult Execute(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            Logger.LogWarning(
                "Activity '{ActivityName}' is no longer available. This can happen if the feature providing the activity is no longer enabled. Either enable the feature, or remove this activity from workflow definition with ID {WorkflowTypeId}",
                MissingActivityRecord.TypeName, workflowContext.WorkflowType.Id);
            return Noop();
        }

        public MissingActivity(IServiceProvider serviceProvider, ActivityRecord missingActivityRecord) : base(
            serviceProvider)
        {
            MissingActivityRecord = missingActivityRecord;
        }
    }
}
