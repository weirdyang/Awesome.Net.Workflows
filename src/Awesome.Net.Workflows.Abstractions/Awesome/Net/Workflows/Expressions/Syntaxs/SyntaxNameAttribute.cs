using System;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp;

namespace Awesome.Net.Workflows.Expressions.Syntaxs
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class SyntaxNameAttribute : Attribute
    {
        public string Name { get; }

        public SyntaxNameAttribute([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            Name = name;
        }

        public static string GetSyntax(Type filterType)
        {
            var filterNameAttribute = filterType
                .GetCustomAttributes(true)
                .OfType<SyntaxNameAttribute>()
                .FirstOrDefault();

            if(filterNameAttribute != null)
            {
                return filterNameAttribute.Name;
            }

            throw new Exception($"Type {filterType.Name} undefined [SyntaxNameAttribute].");
        }
    }
}