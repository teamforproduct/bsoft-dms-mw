using System;

namespace BL.CrossCutting.Helpers
{
    public class FileLogger
    {

        public static void AppendTextToFile(string text, string FilePath, Exception ex = null)
        {
            try
            {
                System.IO.StreamWriter sw;

                try
                {
                    System.IO.FileInfo ff = new System.IO.FileInfo(FilePath);
                    if (!ff.Exists)
                    {
                        //ff.Create();
                    }
                }
                catch (Exception e)
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
                catch (Exception e)
                {
                }
                finally
                {
                    sw.Close();
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
