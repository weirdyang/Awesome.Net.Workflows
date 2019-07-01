using JetBrains.Annotations;
using Microsoft.Extensions.Localization;

namespace Awesome.Net.Workflows.Models
{
    public class Outcome
    {
        public Outcome(LocalizedString displayName) : this(displayName.Name, displayName)
        {
        }


        public Outcome(string name, LocalizedString displayName = null)
        {
            Name = name;
            DisplayName = displayName ?? new LocalizedString(name, name);
        }

        [NotNull]
        public string Name { get; }

        [NotNull]
        public LocalizedString DisplayName { get; }
    }
}
