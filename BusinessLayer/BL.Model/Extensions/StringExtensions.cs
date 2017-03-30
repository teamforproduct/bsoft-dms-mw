using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveSpecialCharactersFullText(this string s)
        {
            return s.Replace("(", string.Empty).Replace(")", string.Empty)
                    .Replace("[", string.Empty).Replace("]", string.Empty)
                    .Replace("[", string.Empty).Replace("]", string.Empty)
                    .Replace("^", string.Empty)
                ;
        }
    }
}
