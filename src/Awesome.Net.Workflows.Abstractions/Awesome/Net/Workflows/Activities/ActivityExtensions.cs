using System;
using Awesome.Net.Workflows.Models;
using Volo.Abp.Localization;

namespace Awesome.Net.Workflows.Activities
{
    public static class ActivityExtensions
    {
        public static bool IsEvent(this IActivity activity)
        {
            return activity is IEvent;
        }

        public static ILocalizableString GetTitleOrDefault(this IActivity activity, Func<ILocalizableString> defaultTitle)
        {
            var title = activity.As<ActivityMetadata>().Title;
            return !string.IsNullOrEmpty(title) ? new FixedLocalizableString(title) : defaultTitle();
        }
    }
}
