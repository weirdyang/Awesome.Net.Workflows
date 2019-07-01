using System;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp;

namespace Awesome.Net.Liquid
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class LiquidFilterNameAttribute : Attribute
    {
        public string Name { get; }

        public LiquidFilterNameAttribute([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            Name = name;
        }

        public static string GetFilterName(Type filterType)
        {
            var filterNameAttribute = filterType
                .GetCustomAttributes(true)
                .OfType<LiquidFilterNameAttribute>()
                .FirstOrDefault();

            if(filterNameAttribute != null)
            {
                return filterNameAttribute.Name;
            }

            return filterType.Name.RemovePostFix("Filter").ToSentenceCase("_").ToCamelCase();
        }
    }
}