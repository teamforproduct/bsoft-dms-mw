using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.Exception;
using DMS_WebAPI.Utilities;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Infrastructure
{
    [AttributeUsage(AttributeTargets.All)]
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var currentContext = HttpContext.Current;
            currentContext.Response.Clear();
            currentContext.Response.StatusCode = (int)HttpStatusCode.OK;
            //TODO Remove
            if (currentContext.IsDebuggingEnabled)
            {
                currentContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            currentContext.Response.ContentType = "application/json";
            if (context.Exception is DmsExceptions)
            {
                if (context.Exception is UserUnauthorized)
                {
                    currentContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
            else
            {

            }

            var json = JsonConvert.SerializeObject(new { success = false, msg = context.Exception.Message }, GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);
            try
            {
                IContext cxt = null;
                try
                {
                    cxt = DmsResolver.Current.Get<UserContext>().Get();
                }
                catch { }
                if (currentContext.User.Identity.IsAuthenticated && cxt != null)
                {
                    var service = DmsResolver.Current.Get<IAdminService>();
                    json = service.ReplaceLanguageLabel(cxt, json);
                }
                else
                {
                    json = new Languages().ReplaceLanguageLabel(currentContext.Request.UserLanguages?[0], json);
                }
            }
            catch { }

            currentContext.Response.Write(json);
            currentContext.Response.End();
            #region log to file
            try
            {
                var ex = context.Exception;

                if (ex.InnerException != null) ex = ex.InnerException;

                // stores the error message
                string errorMessage = string.Empty;
                errorMessage += "ERROR!!!\r\n";
                try
                {
                    HttpContext cnt = currentContext;
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
                    currentContext.Request.InputStream.Position = 0;
                    errorMessage += $"Request Body:{new System.IO.StreamReader(currentContext.Request.InputStream).ReadToEnd()}\r\n";
                }
                catch { }


                try
                {
                    System.IO.StreamWriter sw;
                    string path = currentContext.Server.MapPath("~/SiteErrors.txt");
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
            #endregion log to file
        }
    }
}
