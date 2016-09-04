using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.CrossCutting.Extensions
{
    public static class StringExtensions
    {

        public static bool ContainsArray(this string s, IEnumerable<string> stringArray)
        {

            if (stringArray == null) return false;

            if (stringArray.Count() == 0) return false;

            foreach (var item in stringArray)
            {
                if (!s.Contains(item)) return false;
            }

            return true;
        }

    }

}
