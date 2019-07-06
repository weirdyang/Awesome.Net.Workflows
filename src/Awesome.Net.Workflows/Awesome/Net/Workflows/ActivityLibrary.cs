using System;
using System.Collections.Generic;
using System.Linq;
using Awesome.Net.Workflows.Activities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Awesome.Net.Workflows
{
    public class ActivityLibrary : IActivityLibrary
    {
        private readonly Lazy<IDictionary<string, IActivity>> _activityDictionary;
        private readonly Lazy<IList<LocalizedString>> _activityCategories;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActivityLibrary> _logger;

        public ActivityLibrary(WorkflowOptions workflowOptions,
            IServiceProvider serviceProvider,
            ILogger<ActivityLibrary> logger)
        {
            _activityDictionary = new Lazy<IDictionary<string, IActivity>>(
                () => workflowOptions.ActivityTypes
                    .Where(x => !x.IsAbstract)
                    .Select(x => serviceProvider.CreateInstance<IActivity>(x))
                    .OrderBy(x => x.TypeName)
                    .ToDictionary(x => x.TypeName));

            _activityCategories = new Lazy<IList<LocalizedString>>(
                () => _activityDictionary.Value.Values
                    .OrderBy(x => x.Category.Value)
                    .Select(x => x.Category)
                    .Distinct(new LocalizedStringComparer())
                    .ToList());

            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        private IDictionary<string, IActivity> ActivityDictionary => _activityDictionary.Value;
        private IEnumerable<LocalizedString> ActivityCategories => _activityCategories.Value;

        public IEnumerable<IActivity> ListActivities()
        {
            return ActivityDictionary.Values;
        }

        public IEnumerable<LocalizedString> ListCategories()
        {
            return ActivityCategories;
        }

        public IActivity GetActivityByName(string typeName)
        {
            return ActivityDictionary.ContainsKey(typeName) ? ActivityDictionary[typeName] : null;
        }

        public IActivity InstantiateActivity(string typeName)
        {
            var activityType = GetActivityByName(typeName)?.GetType();

            if (activityType == null)
            {
                _logger.LogWarning(
                    "Requested activity '{ActivityName}' does not exist in the library. This could indicate a changed typeName or a missing feature.",
                    typeName);
                return null;
            }

            return InstantiateActivity(activityType);
        }

        public IEnumerable<IActivity> InstantiateActivities(IEnumerable<string> activityTypeNames)
        {
            var activityNameList = activityTypeNames.ToList();
            foreach (var activitySample in ActivityDictionary.Values.Where(x => activityNameList.Contains(x.TypeName)))
            {
                yield return InstantiateActivity(activitySample.GetType());
            }
        }

        private IActivity InstantiateActivity(Type activityType)
        {
            return _serviceProvider.CreateInstance<IActivity>(activityType);
        }
    }
}
