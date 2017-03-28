using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.CrossCutting.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> IfNullThenEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static string ListToString(this IEnumerable<DateTime?> source)
        {
            var res = "";
            if (source?.Any() ?? false)
                source.ToList().ForEach(x => { if (x.HasValue) res = res + " " + x.Value.ToString("dd.MM.yyyy"); });
            return res;
        }

    }
}