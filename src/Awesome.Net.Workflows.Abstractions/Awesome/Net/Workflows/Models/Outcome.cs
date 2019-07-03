using Awesome.Net.Workflows.Converters;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

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

        public string Name { get; }

        [JsonConverter(typeof(LocalizedStringConverter))]
        public LocalizedString DisplayName { get; }
    }
}
