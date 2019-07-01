using Awesome.Net.Workflows.Activities;

namespace Awesome.Net.Workflows.Models
{
    public class ActivityContext
    {
        public ActivityRecord ActivityRecord { get; set; }
        public IActivity Activity { get; set; }
    }
}