using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Models;

namespace Awesome.Net.Workflows.Contexts
{
    public class ActivityExecutionContext
    {
        public ActivityRecord ActivityRecord { get; set; }
        public IActivity Activity { get; set; }
    }
}
