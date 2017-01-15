﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.CrossCutting.Helpers
{
    public class FileLogger
    {
        
        public static void AppendTextToSiteErrors(string method, TimeSpan elapsed)
        {
            AppendTextToSiteErrors($"{method};{System.DateTime.UtcNow.ToString("o")};{String.Format("{0:0.00000000}", elapsed.TotalSeconds)}");
        }

        public static void AppendTextToSiteErrors(string text)
        {
            AppendTextToFile(text, System.Web.HttpContext.Current.Server.MapPath("~/SiteErrors.txt"));
        }

        public static void AppendTextToFile(string text, string FilePath, Exception ex = null)
        {
            try
            {
                System.IO.StreamWriter sw;

                try
                {
                    System.IO.FileInfo ff = new System.IO.FileInfo(FilePath);
                    if (ff.Exists)
                    {
                    }
                }
                catch
                {

                }

                sw = System.IO.File.AppendText(FilePath);
                try
                {
                    sw.WriteLine(text);
                    if (ex != null)
                    {
                        sw.WriteLine(ex.StackTrace);
                    }
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
