namespace Awesome.Net.Workflows.Models
{
    public class BlockingActivity
    {
        public static BlockingActivity FromActivity(ActivityRecord activity)
        {
            return new BlockingActivity
            {
                Id = activity.Id, IsStart = activity.IsStart, Name = activity.TypeName
            };
        }

        public string Id { get; set; }
        public bool IsStart { get; set; }
        public string Name { get; set; }
    }
}
