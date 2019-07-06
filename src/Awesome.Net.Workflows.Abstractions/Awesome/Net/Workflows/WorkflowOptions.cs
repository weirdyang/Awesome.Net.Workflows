using System;
using System.Collections.Generic;
using System.Linq;
using Awesome.Net.Workflows.Activities;
using Microsoft.Extensions.DependencyInjection;

namespace Awesome.Net.Workflows
{
    public class WorkflowOptions
    {
        /// <summary>
        /// The set of activities available to workflows.
        /// Modules can register and unregister activities.
        /// </summary>
        public IDictionary<Type, Type> ActivityDictionary { get; } = new Dictionary<Type, Type>();

        public IEnumerable<Type> ActivityTypes => ActivityDictionary.Values.ToList().AsReadOnly();

        public IServiceCollection Services { get; }

        public WorkflowOptions(IServiceCollection services)
        {
            Services = services;
        }
    }

    public static class WorkflowOptionsExtensions
    {
        public static WorkflowOptions RegisterActivity<T>(this WorkflowOptions options) where T : IActivity
        {
            if (!options.IsActivityRegistered<T>())
            {
                var activityType = typeof(T);
                options.ActivityDictionary.Add(activityType, activityType);
            }

            return options;
        }

        public static WorkflowOptions UnregisterActivityType<T>(this WorkflowOptions options) where T : IActivity
        {
            if (!options.IsActivityRegistered<T>())
                throw new InvalidOperationException("The specified activity type is not registered.");

            options.ActivityDictionary.Remove(typeof(T));
            return options;
        }

        public static bool IsActivityRegistered<T>(this WorkflowOptions options) where T : IActivity
        {
            return options.ActivityDictionary.ContainsKey(typeof(T));
        }
    }
}
