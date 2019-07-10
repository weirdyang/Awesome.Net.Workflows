using System;
using System.Collections.Generic;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Contexts;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;
using NCrontab;

namespace Awesome.Net.Workflows.Timers
{
    public class TimerEvent : EventActivity
    {
        private IDateTimeProvider _dateTimeProvider;

        public TimerEvent(IServiceProvider serviceProvider, IDateTimeProvider dateTimeProvider) : base(serviceProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public override LocalizedString Category => T["Background"];

        public string CronExpression
        {
            get => GetProperty(() => "*/5 * * * *");
            set => SetProperty(value);
        }

        public DateTime? StartAtUtc
        {
            get => GetProperty<DateTime?>();
            set => SetProperty(value);
        }

        private DateTime? StartedUtc
        {
            get => GetProperty<DateTime?>();
            set => SetProperty(value);
        }

        public override bool CanExecute(WorkflowExecutionContext workflowContext, ActivityExecutionContext activityContext)
        {
            return StartedUtc == null || IsExpired();
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            return Outcomes(T["Done"]);
        }

        public override ActivityExecutionResult Resume(WorkflowExecutionContext workflowContext,
            ActivityExecutionContext activityContext)
        {
            if (IsExpired())
            {
                workflowContext.LastResult = "TimerEvent";
                return Outcomes("Done");
            }

            return Halt();
        }

        private bool IsExpired()
        {
            if (StartedUtc == null)
            {
                StartedUtc = StartAtUtc ?? _dateTimeProvider.Now;
            }

            var schedule = CrontabSchedule.Parse(CronExpression);
            var whenUtc = schedule.GetNextOccurrence(StartedUtc.Value);

            return _dateTimeProvider.Now >= whenUtc;
        }
    }
}
