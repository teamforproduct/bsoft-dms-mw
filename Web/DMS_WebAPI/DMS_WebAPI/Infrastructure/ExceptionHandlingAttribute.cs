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
using System.Collections.Generic;
using DMS_WebAPI.Models;

namespace DMS_WebAPI.Infrastructure
{
    [AttributeUsage(AttributeTargets.All)]
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var httpContext = HttpContext.Current;
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            //TODO Remove
            if (httpContext.IsDebuggingEnabled)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            httpContext.Response.ContentType = "application/json";

            if (context.Exception is DmsExceptions)
            {
                if (context.Exception is UserUnauthorized)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
            else
            {

            }

            var exc = context.Exception;
            

            string msgExp = string.Empty;

            
            
            //#if DEBUG
            //pss Убрать в продакшине, пока для понимания вопроса во время разработки пусть отображается полная информация!!!
            while (exc != null)
            {
                var m = exc.Message;

                m = GetTranslation(m);

                if (exc is DmsExceptions)
                {
                    var p = (exc as DmsExceptions).Parameters;

                    if (p?.Count > 0) m = InsertValues(m, p);
                }

                if (!m.Contains("See the inner exception for details"))
                {
                    msgExp = msgExp + (msgExp == string.Empty ? string.Empty : ";    ") + m;
                }
                exc = exc.InnerException;
            };

            // Если в результате подстановки параметров подставили лейблы, нужно их перевести
            msgExp = GetTranslation(msgExp);

            //#else
            //msgExp = exc.Message;
            //#endif

            var settings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;

            var json = JsonConvert.SerializeObject(new { success = false, msg = msgExp }, settings);
            //json = ReplaceLanguageLabel(currentContext, json);


            httpContext.Response.Write(json);
            httpContext.Response.End();



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
                    HttpContext cnt = httpContext;
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
                    httpContext.Request.InputStream.Position = 0;
                    errorMessage += $"Request Body:{new System.IO.StreamReader(httpContext.Request.InputStream).ReadToEnd()}\r\n";
                }
                catch { }


                try
                {
                    System.IO.StreamWriter sw;
                    string path = httpContext.Server.MapPath("~/SiteErrors.txt");
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
                        string line = DateTime.UtcNow.ToString("o") + "\r\n" + errorMessage;
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

        

        private string InsertValues(string Message, List<string> Paramenters)
        {
            try
            {
                return string.Format(Message, Paramenters.ToArray());
            }
            catch
            { }
            return Message;
        }

        private string GetTranslation(string text)
        {
            var httpContext = HttpContext.Current;
            IContext defContext = null;

            try
            {
                defContext = DmsResolver.Current.Get<UserContexts>().Get();
            }
            catch 
            { }

            var languageService = DmsResolver.Current.Get<ILanguages>();
            if (defContext == null) return languageService.ReplaceLanguageLabel(httpContext, text);
            else return languageService.ReplaceLanguageLabel(defContext, text);
        }
    }
}
