using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.CrossCutting.Extensions
{
    public static class StringExtensions
    {

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) { return value; }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }

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

        public static string Compress(this string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static string Decompress(this string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }
        public static string RemoveSpecialCharactersFullText(this string s)
        {
            return s.Replace("(", string.Empty).Replace(")", string.Empty)
                    .Replace("[", string.Empty).Replace("]", string.Empty)
                    .Replace("[", string.Empty).Replace("]", string.Empty)
                    .Replace("^", string.Empty).Replace("]", string.Empty)
                ;
        }



    }

}
