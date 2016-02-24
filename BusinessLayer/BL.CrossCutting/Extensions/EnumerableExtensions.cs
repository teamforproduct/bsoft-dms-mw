using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> IfNullThenEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}