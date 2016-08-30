using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.Common
{
    internal static class RecursiveExtension
    {
        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recursiveFunc)
        {
            if (source != null)
            {
                foreach (var mainItem in source)
                {
                    yield return mainItem;

                    IEnumerable<T> recursiveSequence = (recursiveFunc(mainItem) ?? new T[] { }).SelectRecursive(recursiveFunc);

                    if (recursiveSequence != null)
                    {
                        foreach (var recursiveItem in recursiveSequence)
                        {
                            yield return recursiveItem;
                        }
                    }
                }
            }
        }

    }
}
