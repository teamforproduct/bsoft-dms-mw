using BL.Model.Exception;
using System;
using System.Net;
using System.Web;
using System.Web.Http.Filters;

namespace DMS_WebAPI.Infrastructure
{
    [AttributeUsage(AttributeTargets.All)]
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.OK;
            //TODO Remove
            if (System.Web.HttpContext.Current.IsDebuggingEnabled)
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }          
            HttpContext.Current.Response.ContentType = "application/json";
            if (context.Exception is DmsExceptions)
            {
                if (context.Exception is UserUnauthorized)
                {
                    HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
            else
            {

            }
            HttpContext.Current.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { success = false, msg = context.Exception.Message }));
            HttpContext.Current.Response.End();
            try
            {
                var ex = context.Exception;

                if (ex.InnerException != null) ex = ex.InnerException;

                // stores the error message
                string errorMessage = string.Empty;
                errorMessage += "ERROR!!!\r\n";
                try
                {
                    HttpContext cnt = HttpContext.Current;
                    errorMessage += $"URL:{cnt.Request.Url.ToString()}\r\n";
                }
                catch
                { }

                errorMessage += $"Message:{ex.Message}\r\n";
                errorMessage += $"Source:{ex.Source}\r\n";
                errorMessage += $"Method:{ex.TargetSite}\r\n";
                errorMessage += $"StackTrace:{ex.StackTrace}\r\n";
                errorMessage += $"Request:{context.Request}\r\n";

                try
                {
                    HttpContext.Current.Request.InputStream.Position = 0;
                    errorMessage += $"Request Body:{new System.IO.StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd()}\r\n";
                }
                catch { }


                try
                {
                    System.IO.StreamWriter sw;
                    string path = HttpContext.Current.Server.MapPath("~/SiteErrors.txt");
                    try
                    {
                        System.IO.FileInfo ff = new System.IO.FileInfo(path);
                        if (ff.Exists)
                        {
                        }
                    }
                    catch
                    {

                    }

                    sw = System.IO.File.AppendText(path);
                    try
                    {
                        string line = DateTime.Now.ToString("o") + "\r\n" + errorMessage;
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
            catch { }
        }
    }
}
