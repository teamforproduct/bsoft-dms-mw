using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.CrossCutting.Helpers
{
    public class Logger
    {
        public static void SaveToFile2(string text)
        {
            try
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(System.Web.HttpContext.Current.Server.MapPath("~/SiteLog.txt"));
                try
                {
                    string line = $"{System.DateTime.UtcNow.ToString("o")};{text};";
                    sw.WriteLine(line);
                }
                catch
                {
                }
                finally
                {
                    sw.Close();
                }
            }
            catch
            {
            }
        }
        public static void SaveToFile(string method, TimeSpan elapsed)
        {
            SaveToFile(method, String.Format("{0:0.00000000}", elapsed.TotalSeconds));
        }
        private static void SaveToFile(string method, string time)
        {
            try
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(System.Web.HttpContext.Current.Server.MapPath("~/SiteLog.txt"));
                try
                {
                    string line = $"{method};{System.DateTime.UtcNow.ToString("o")};{time}";
                    sw.WriteLine(line);
                }
                catch
                {
                }
                finally
                {
                    sw.Close();
                }
            }
            catch
            {
            }
        }
    }
}
