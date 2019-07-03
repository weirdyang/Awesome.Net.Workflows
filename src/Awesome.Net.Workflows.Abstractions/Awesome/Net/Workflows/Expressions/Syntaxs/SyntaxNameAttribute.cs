using System;
using System.Linq;

namespace Awesome.Net.Workflows.Expressions.Syntaxs
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class SyntaxNameAttribute : Attribute
    {
        public string Name { get; }

        public SyntaxNameAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            }

            Name = name;
        }

        public static string GetSyntax(Type filterType)
        {
            var filterNameAttribute = filterType
                .GetCustomAttributes(true)
                .OfType<SyntaxNameAttribute>()
                .FirstOrDefault();

            if (filterNameAttribute != null)
            {
                return filterNameAttribute.Name;
            }

            throw new Exception($"Type {filterType.Name} undefined [SyntaxNameAttribute].");
        }
    }
}
