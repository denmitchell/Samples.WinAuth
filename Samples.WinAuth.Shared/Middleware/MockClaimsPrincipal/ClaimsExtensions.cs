using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Samples.WinAuth.Shared
{

    /// <summary>
    /// Provides extensions for translating to/from IEnumerable`Claim
    /// </summary>
    public static class ClaimsExtensions
    {

        /// <summary>
        /// Convert a dictionary to an IEnumerable`Claim
        /// </summary>
        /// <typeparam name="E">type that can hold an IEnumerable`string </typeparam>
        /// <param name="dict">the claims as a dictionary</param>
        /// <returns>collection of claims</returns>
        public static IEnumerable<Claim> ToClaimEnumerable<E>(
                this Dictionary<string, E> dict)
                where E : IEnumerable<string>
        {
            var list = new List<Claim>();
            foreach (var entry in dict)
            {
                foreach (var item in entry.Value)
                    list.Add(new Claim(entry.Key, item));
            }
            return list;
        }

        /// <summary>
        /// Convert a collection of Claims into a dictionary
        /// </summary>
        /// <param name="claims">The claims to convert</param>
        /// <returns>dictionary of claims</returns>
        public static Dictionary<string, string[]> ToStringDictionary(this IEnumerable<Claim> claims)
        {
            return claims.GroupBy(c => new { c.Type })
             .Select(g => new { g.Key.Type, Value = g.Select(i => i.Value).ToArray() })
                 .ToDictionary(d => d.Type, d => d.Value);
        }

        /// <summary>
        /// Convert a collection of Claims into a dictionary for use with
        /// Logger.BeginScope
        /// </summary>
        /// <param name="claims">The claims to convert</param>
        /// <returns>dictionary of claims</returns>
        public static Dictionary<string, object> ToLoggerScope(this IEnumerable<Claim> claims)
        {
            return claims.GroupBy(c => new { c.Type })
             .Select(g => new { g.Key.Type, Value = g.Select(i => i.Value).ToArray() })
                 .ToDictionary(d => d.Type, d => (object)d.Value);
        }

    }
}
