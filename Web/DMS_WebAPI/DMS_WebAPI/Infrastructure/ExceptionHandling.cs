using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Exception;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace DMS_WebAPI.Infrastructure
{
    public static class ExceptionHandling
    {
        private static int GetResponseStatusCode(HttpContext context, Exception exception)
        {
            var res = (int)HttpStatusCode.OK;

            //TODO Remove
            if (context.IsDebuggingEnabled)
            {
                res = (int)HttpStatusCode.InternalServerError;
            }

            if (exception is DmsExceptions)
            {
                if (exception is UserUnauthorized)
                {
                    res = (int)HttpStatusCode.Unauthorized;
                }
            }

            return res;
        }

        public static void ReturnExceptionResponse(Exception exception, HttpActionExecutedContext context = null)
        {

            var url = string.Empty;
            var body = string.Empty;
            var request = string.Empty;
            var responceExpression = string.Empty;
            var logExpression = string.Empty;
            var exc = exception;

            #region [+] Формирование текста исключения, лога ...

            //#if DEBUG
            //pss Убрать в продакшине, пока для понимания вопроса во время разработки пусть отображается полная информация!!!
            while (exc != null)
            {
                var m = exc.Message;

                if (!m.Contains("See the inner exception for details"))
                {
                    logExpression += (logExpression == string.Empty ? "Exception:" : "InnerException:") + "\r\n";
                    logExpression += $"   Message: {exc.Message}\r\n";
                    logExpression += $"   Source: {exc.Source}\r\n";
                    logExpression += $"   Method: {exc.TargetSite}\r\n";
                }

                m = GetTranslation(m);

                if (exc is DmsExceptions)
                {
                    var p = (exc as DmsExceptions).Parameters;

                    if (p?.Count > 0) m = InsertValues(m, p);
                }

                if (!m.Contains("See the inner exception for details"))
                {
                    responceExpression = responceExpression + (responceExpression == string.Empty ? string.Empty : ";    ") + m;
                }
                exc = exc.InnerException;
            };

            // Если в результате подстановки параметров подставили лейблы, нужно их перевести
            responceExpression = GetTranslation(responceExpression);

            //#else
            //msgExp = exc.Message;
            //#endif

            #endregion

            var settings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            var json = JsonConvert.SerializeObject(new { success = false, msg = responceExpression }, settings);

            #region [+] Получение параметров текущего HttpContext ...

            var httpContext = HttpContext.Current;

            if (context != null)
            request = $"{context.Request}";

            try { url = httpContext.Request.Url.ToString(); } catch { }

            try
            {
                httpContext.Request.InputStream.Position = 0;
                body = new System.IO.StreamReader(httpContext.Request.InputStream).ReadToEnd();
            }
            catch { }
            #endregion

            #region [+] Формирование Response ...
            // Очищаю существующий Response
            try
            {
                httpContext.Response.Clear();

                httpContext.Response.ContentType = "application/json";
                // Здесь может возникнуть исключение Server cannot set status after HTTP headers have been sent.
                // при повторнов входе из Application_Error если раскоментировать httpContext.Response.End(); 
                httpContext.Response.StatusCode = GetResponseStatusCode(httpContext, exception);
                httpContext.Response.Write(json);
                // Этот End очень важен для фронта. без него фронт получает статус InternalServerError на ошибке UserUnauthorized. НЕ понятно 
                httpContext.Response.End();
            }
            catch { }
            #endregion


            #region log to file
            try
            {
                // stores the error message
                string errorMessage = string.Empty;
                errorMessage += "ERROR!!! - " + DateTime.UtcNow.ToString("o") + "\r\n";

                errorMessage += $"URL: {url}\r\n";
                //errorMessage += $"Message:{ex.Message}\r\n";
                //errorMessage += $"Source:{ex.Source}\r\n";
                //errorMessage += $"Method:{ex.TargetSite}\r\n";
                //errorMessage += $"StackTrace:{ex.StackTrace}\r\n";
                errorMessage += logExpression;

                

                // Этот иф мне не понятен. Почему StackTrace нужно пытаться брать из InnerException
                if (exception.InnerException != null)
                    exc = exception.InnerException;
                else
                    exc = exception;

                errorMessage += $"StackTrace:\r\n{exc.StackTrace}\r\n";

                errorMessage += $"Request:\r\n{request}\r\n";

                errorMessage += $"Request Body: {body}\r\n";


                AppendToFile(httpContext.Server.MapPath("~/SiteErrors.txt"), errorMessage);
            }
            catch { }
            #endregion log to file

        }



        private static string InsertValues(string Message, List<string> Paramenters)
        {
            try
            {
                return string.Format(Message, Paramenters.ToArray());
            }
            catch
            { }
            return Message;
        }

        private static string GetTranslation(string text)
        {
            var languageService = DmsResolver.Current.Get<ILanguages>();
            return languageService.GetTranslation(text);
        }

        private static void AppendToFile(string path, string text)
        {
            try
            {
                System.IO.StreamWriter sw;

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
                    string line = text;
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