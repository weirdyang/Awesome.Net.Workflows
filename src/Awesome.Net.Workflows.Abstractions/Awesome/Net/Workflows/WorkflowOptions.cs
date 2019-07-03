using System;
using System.Collections.Generic;
using System.Linq;

namespace Awesome.Net.Workflows
{
    public class WorkflowOptions
    {
        /// <summary>
        /// The set of activities available to workflows.
        /// Modules can register and unregister activities.
        /// </summary>
        private IDictionary<Type, Type> ActivityDictionary { get; } = new Dictionary<Type, Type>();

        public IEnumerable<Type> ActivityTypes => ActivityDictionary.Values.ToList().AsReadOnly();

        public WorkflowOptions RegisterActivity(Type activityType)
        {
            if(!ActivityDictionary.ContainsKey(activityType))
            {
                ActivityDictionary.Add(activityType, activityType);
            }

            return this;
        }

        public WorkflowOptions UnregisterActivityType(Type activityType)
        {
            if(!ActivityDictionary.ContainsKey(activityType))
                throw new InvalidOperationException("The specified activity type is not registered.");

            ActivityDictionary.Remove(activityType);
            return this;
        }

        public bool IsActivityRegistered(Type activityType)
        {
            return ActivityDictionary.ContainsKey(activityType);
        }
    }

    public static class WorkflowOptionsExtensions
    {
        public static WorkflowOptions RegisterActivityType<T>(this WorkflowOptions options)
        {
            return options.RegisterActivity(typeof(T));
        }

        public static WorkflowOptions RegisterActivity<T>(this WorkflowOptions options)
        {
            return options.RegisterActivity(typeof(T));
        }

        public static WorkflowOptions UnregisterActivityType<T>(this WorkflowOptions options)
        {
            return options.UnregisterActivityType(typeof(T));
        }

        public static bool IsActivityRegistered<T>(this WorkflowOptions options)
        {
            return options.IsActivityRegistered(typeof(T));
        }
    }
}
