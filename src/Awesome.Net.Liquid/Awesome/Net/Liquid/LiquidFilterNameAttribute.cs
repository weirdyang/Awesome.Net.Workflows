using System;
using System.Linq;

namespace Awesome.Net.Liquid
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class LiquidFilterNameAttribute : Attribute
    {
        public string Name { get; }

        public LiquidFilterNameAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            Name = name;
        }

        public static string GetName(Type filterType)
        {
            var filterNameAttribute = filterType
                .GetCustomAttributes(true)
                .OfType<LiquidFilterNameAttribute>()
                .FirstOrDefault();

            if (filterNameAttribute != null)
            {
                return filterNameAttribute.Name;
            }

            return filterType.Name.RemovePostFix("Filter").ToSentenceCase("_").ToCamelCase();
        }
    }
}