using System;

namespace Awesome.Net.Workflows.Activities
{
    public abstract class TaskActivity : Activity, ITask
    {
        protected TaskActivity(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
