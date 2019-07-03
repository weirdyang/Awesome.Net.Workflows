using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace Awesome.Net.Workflows
{
    public class LocalizedStringComparer : IEqualityComparer<LocalizedString>
    {
        public bool Equals(LocalizedString x, LocalizedString y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(LocalizedString obj)
        {
            return obj.GetHashCode();
        }
    }
}