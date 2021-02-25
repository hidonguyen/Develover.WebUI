using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Develover.WebUI.Extensions
{
    public static class GuidExtensions
    {
        public static string ToLowerString(this Guid @this)
        {
            return @this.ToString().ToLower();
        }
        public static string ToLowerString(this Guid? @this)
        {
            if (@this == null)
                return Guid.Empty.ToString();
            return @this.ToString().ToLower();
        }
    }
}
