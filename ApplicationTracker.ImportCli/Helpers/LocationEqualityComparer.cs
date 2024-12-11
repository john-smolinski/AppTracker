using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTracker.ImportCli.Helpers
{
    public class LocationEqualityComparer : IEqualityComparer<KeyValuePair<string?, string>>
    {
        public bool Equals(KeyValuePair<string?, string> x, KeyValuePair<string?, string> y)
        {
            return string.Equals(x.Key, y.Key, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(KeyValuePair<string?, string> obj)
        {
            return HashCode.Combine(
                obj.Key?.ToLowerInvariant(),
                obj.Value?.ToLowerInvariant()
            );
        }
    }
}
