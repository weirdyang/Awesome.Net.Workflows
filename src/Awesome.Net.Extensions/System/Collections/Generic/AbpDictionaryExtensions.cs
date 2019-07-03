using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;

namespace System.Collections.Generic
{
    /// <summary>
    /// Extension methods for Dictionary.
    /// </summary>
    public static class AbpDictionaryExtensions
    {
        /// <summary>
        /// This method is used to try to get a value in a dictionary if it does exists.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="dictionary">The collection object</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value of the key (or default value if key not exists)</param>
        /// <returns>True if key does exists in the dictionary</returns>
        internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

            if (dictionary.TryGetValue(key, out var valueObj) && valueObj is T)
            {
                value = (T) valueObj;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var obj) ? obj : default;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var obj) ? obj : default;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var obj) ? obj : default;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var obj) ? obj : default;
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TKey, TValue> factory)
        {
            if (dictionary.TryGetValue(key, out var obj)) return obj;

            return dictionary[key] = factory(key);
        }

        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> factory)
        {
            return dictionary.GetOrAdd(key, k => factory());
        }

        /// <summary>
        /// Merges the specified other dictionary into this dictionary.
        /// Existing entries are overwritten.
        /// </summary>
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            IDictionary<TKey, TValue> other)
        {
            var copy = new Dictionary<TKey, TValue>(dictionary);
            foreach (var item in other) copy[item.Key] = item.Value;
            return copy;
        }

        public static T As<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                var key = Char.ToUpper(item.Key[0]) + item.Key.Substring(1);
                var targetProperty = someObjectType.GetProperty(key);

                if (targetProperty != null)
                {
                    if (targetProperty.PropertyType == typeof(string))
                    {
                        targetProperty.SetValue(someObject, item.Value);
                    }
                    else
                    {
                        var parseMethod = targetProperty.PropertyType.GetMethod("TryParse",
                            BindingFlags.Public | BindingFlags.Static, null,
                            new[] {typeof(string), targetProperty.PropertyType.MakeByRefType()}, null);

                        if (parseMethod != null)
                        {
                            var parameters = new[] {item.Value, null};
                            var success = (bool) parseMethod.Invoke(null, parameters);
                            if (success) targetProperty.SetValue(someObject, parameters[1]);
                        }
                    }
                }
            }

            return someObject;
        }
        
        public static dynamic AsDynamic(this IDictionary<string, object> dictionary)
        {
            var expandoObj = new ExpandoObject();
            var expandoObjCollection = (ICollection<KeyValuePair<string, object>>) expandoObj;

            foreach (var keyValuePair in dictionary) expandoObjCollection.Add(keyValuePair);
            return expandoObj;
        }
    }
}