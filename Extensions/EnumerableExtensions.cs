using System.Collections.Generic;

namespace Develover.WebUI.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<T> ToSafeList<T>(this IEnumerable<T> source) => new List<T>(source);
    }
}
