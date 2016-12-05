using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
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
        private static HttpStatusCode GetResponseStatusCode(HttpContext context, Exception exception)
        {
            var res = HttpStatusCode.OK;

            //TODO Remove
            if (context.IsDebuggingEnabled)
            {
                res = HttpStatusCode.InternalServerError;
            }

            if (exception is DmsExceptions)
            {
                // Договорились при возникновении этих ошибок отправлять статус Unauthorized. Фронт редиректит пользователя на LoginPage
                if (exception is UserUnauthorized
                    || exception is UserNameOrPasswordIsIncorrect
                    || exception is UserIsDeactivated
                    || exception is UserAccessIsDenied
                    || exception is UserMustConfirmEmail
                    || exception is UserPositionIsNotDefined
                    || exception is UserNameIsNotDefined
                    || exception is DatabaseIsNotSet
                    || exception is UserNotExecuteAnyPosition
                    || exception is UserNotExecuteCheckPosition
                    ) res = HttpStatusCode.Unauthorized;
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
            HttpStatusCode statusCode = HttpStatusCode.OK;
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
                statusCode = GetResponseStatusCode(httpContext, exception);

                httpContext.Response.Clear();

                httpContext.Response.ContentType = "application/json";
                // Здесь может возникнуть исключение Server cannot set status after HTTP headers have been sent.
                // при повторнов входе из Application_Error если раскоментировать httpContext.Response.End(); 
                httpContext.Response.StatusCode = (int)statusCode;
                httpContext.Response.Write(json);
                // Этот End очень важен для фронта. без него фронт получает статус InternalServerError на ошибке UserUnauthorized. НЕ понятно 
                httpContext.Response.End();
            }
            catch { }
            #endregion

            #region [+] Запись расширенных параметров ошибки в файл ...
            try
            {
                // stores the error message
                string errorMessage = string.Empty;
                errorMessage += "ERROR!!! - " + DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm") + " UTC\r\n";

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

                FileLogger.AppendTextToSiteErrors(errorMessage);
            }
            catch { }
            #endregion log to file


            // Если возникли ошибки не совместивмые с дальнейшей работой пользователя - удаляю пользовательский контекст
            if (statusCode == HttpStatusCode.Unauthorized)
                DmsResolver.Current.Get<Utilities.UserContexts>().KillCurrentSession();

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



    }
}