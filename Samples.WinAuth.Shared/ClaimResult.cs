using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samples.WinAuth.Shared
{
    public class ClaimResult
    {
        public int StatusCode { get; set; }
        public Dictionary<string,string[]> Claims { get; set; }
    }
}
