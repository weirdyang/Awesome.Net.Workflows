using System.Collections.Generic;
using Awesome.Net.Workflows.Activities;
using Awesome.Net.Workflows.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;

namespace Awesome.Net.Workflows
{
    public interface IActivityLibrary
    {
        /// <summary>
        /// Returns a list of instances of all available <see cref="IActivity"/> implementations.
        /// </summary>
        IEnumerable<IActivity> ListActivities();

        /// <summary>
        /// Returns a list of all available activity categories.
        /// </summary>
        /// <returns></returns>
        IEnumerable<LocalizedString> ListCategories();

        /// <summary>
        /// Returns an activity instance with the specified typeName from the library.
        /// </summary>
        IActivity GetActivityByName(string typeName);

        /// <summary>
        /// Returns a new instance of the activity with the specified typeName.
        /// </summary>
        IActivity InstantiateActivity(string typeName);

        /// <summary>
        /// Returns new instances the specified activities.
        /// </summary>
        IEnumerable<IActivity> InstantiateActivities(IEnumerable<string> activityTypeNames);
    }

    public static class ActivityLibraryExtensions
    {
        public static T InstantiateActivity<T>(this IActivityLibrary library, string typeName) where T : IActivity
        {
            return (T) library.InstantiateActivity(typeName);
        }

        public static T InstantiateActivity<T>(this IActivityLibrary library, string typeName, JObject properties)
            where T : IActivity
        {
            var activity = InstantiateActivity<T>(library, typeName);

            if (activity != null)
            {
                activity.Properties = properties;
            }

            return activity;
        }

        public static T InstantiateActivity<T>(this IActivityLibrary library, ActivityRecord record) where T : IActivity
        {
            return InstantiateActivity<T>(library, record.TypeName, record.Properties);
        }
    }
}
