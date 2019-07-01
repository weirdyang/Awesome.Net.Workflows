using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace Awesome.Net.Data
{
    public static class HasJObjectPropertiesExtensions
    {
        public static bool HasProperty(this IHasJObjectProperties source, string name)
        {
            return source.Properties.ContainsKey(name);
        }


        public static T GetProperty<T>(this IHasJObjectProperties source, Func<T> defaultValue = null, [CallerMemberName]string name = null)
        {
            var item = source.Properties[name];
            return item != null ? item.ToObject<T>() : defaultValue != null ? defaultValue() : default;
        }

        public static T GetProperty<T>(this IHasJObjectProperties source, Type type, Func<T> defaultValue = null, [CallerMemberName]string name = null)
        {
            var item = source.Properties[name];
            return item != null ? (T)item.ToObject(type) : defaultValue != null ? defaultValue() : default;
        }

        public static void SetProperty(this IHasJObjectProperties source, object value, [CallerMemberName]string name = null)
        {
            source.Properties[name] = JToken.FromObject(value);
        }
    }
}