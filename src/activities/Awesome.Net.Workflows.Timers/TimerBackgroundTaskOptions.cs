using System;

namespace Awesome.Net.Workflows.Timers
{
    public class TimerBackgroundTaskOptions
    {
        public TimeSpan PollingTime { get; set; } = TimeSpan.FromMinutes(1);
    }
}
