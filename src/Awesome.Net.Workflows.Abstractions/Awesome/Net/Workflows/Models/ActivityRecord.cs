using Awesome.Net.Workflows.Activities;
using Newtonsoft.Json.Linq;

namespace Awesome.Net.Workflows.Models
{
    public class ActivityRecord
    {
        public string ActivityId { get; set; }

        /// <summary>
        /// The type of the activity.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// The left coordinate of the activity.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The top coordinate of the activity.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Whether the activity is a start state for a <see cref="WorkflowType"/>.
        /// </summary>
        public bool IsStart { get; set; }

        public JObject Properties { get; set; } = new JObject();

        public static ActivityRecord FromActivity(IActivity activity)
        {
            return new ActivityRecord
            {
                IsStart = true,
                Properties = new JObject(activity.Properties),
                TypeName = activity.TypeName
            };
        }
    }
}
