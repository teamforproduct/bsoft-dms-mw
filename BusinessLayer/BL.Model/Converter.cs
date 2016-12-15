using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model
{
    public static class Converter
    {

        public static string ToBase64String(byte[] inArray)
        {
            string fileContect = string.Empty;

            if (inArray != null) fileContect = System.Convert.ToBase64String(inArray);

            return fileContect;
        }
    }
}
