using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Helpers;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Exception;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                    || exception is UserContextIsNotDefined
                    || exception is DatabaseIsNotSet
                    || exception is UserNotExecuteAnyPosition
                    || exception is UserNotExecuteCheckPosition
                    ) res = HttpStatusCode.Unauthorized;
            }


            return res;
        }

        /// <summary>
        /// Получает метку для перевода исключения
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="logExpression"></param>
        /// <param name="descriptionExpression"></param>
        /// <returns></returns>
        private static string GetExceptionText(Exception exception, out string logExpression, out string descriptionExpression)
        {
            var exc = exception;
            var responceExpression = string.Empty;
            logExpression = string.Empty;
            descriptionExpression = string.Empty;
            var languageService = DmsResolver.Current.Get<ILanguages>();
            while (exc != null)
            {
                var m = string.Empty;
                var d = string.Empty;

                // для DmsExceptions Message формирую на основании названия класса
                if (exc is DmsExceptions) m = "##l@DmsExceptions:" + exc.GetType().Name + "@l##";
                else m = exc.Message;

                // ошибки SQL-базы
                if (exc is SqlException)
                {
                    var e = (exc as SqlException);
                    d = e.Message;
                    switch (e.Number)
                    {
                        // The DELETE statement conflicted with the REFERENCE constraint
                        case 547:
                            m = "##l@SqlExceptions:" + "ConflictedWithReferenceConstraint" + "@l##";
                            break;
                        // {"Cannot insert duplicate key row in object '' with unique index ''.
                        case 2601:
                            m = "##l@SqlExceptions:" + "CannotInsertDuplicateKeyRow" + "@l##";
                            break;
                        default:
                            m = $"Number: {e.Number}; Msg: {e.Message}";
                            break;
                    }
                }

                if (m.Contains("See the inner exception for details") || m.Contains("One or more errors occurred"))
                {
                    exc = exc.InnerException;
                    continue;
                }

                // перевожу
                m = languageService.GetTranslation(m);

                // подстановка параметров в сообщение
                if (exc is DmsExceptions)
                {
                    var p = (exc as DmsExceptions).Parameters;

                    if (p?.Count > 0) m = InsertValues(m, p);
                }


                // Без вложенных сообщений
                if (string.IsNullOrEmpty(responceExpression)) responceExpression = m;
                //else
                descriptionExpression = descriptionExpression + (descriptionExpression == string.Empty ? string.Empty : ";    ") + (string.IsNullOrEmpty(d) ? m : d);

                logExpression += (logExpression == string.Empty ? "Exception:" : "InnerException:") + "\r\n";
                logExpression += $"   Message: {(exc is DmsExceptions ? exc.GetType().Name : exc.Message)}\r\n";
                logExpression += $"   Source: {exc.Source}\r\n";
                logExpression += $"   Method: {exc.TargetSite}\r\n";

                exc = exc.InnerException;
            }

            // Если в результате подстановки параметров подставили лейблы, нужно их перевести
            descriptionExpression = languageService.GetTranslation(descriptionExpression);
            responceExpression = languageService.GetTranslation(responceExpression);

            return responceExpression;
        }

        public static void ReturnExceptionResponse(Exception exception, HttpActionExecutedContext context = null)
        {

            bool fromGlobalAsax = context == null;

            var user = string.Empty;
            var url = string.Empty;
            var body = string.Empty;
            var request = string.Empty;

            HttpStatusCode statusCode = HttpStatusCode.OK;
            var exc = exception;
            var logExpression = string.Empty;
            var responceDescription = string.Empty;
            var responceExpression = GetExceptionText(exception, out logExpression, out responceDescription);

            var settings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            var json = JsonConvert.SerializeObject(new { success = false, msg = responceExpression, code = exception.GetType().Name, description = responceDescription }, settings);

            #region [+] Получение параметров текущего HttpContext ...

            var httpContext = HttpContext.Current;

            if (context != null)
            {
                request = $"{context.Request}";
            }

            try { url = httpContext.Request.HttpMethod + " " + httpContext.Request.Url.ToString(); } catch { }

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

                // есть проблема: ошибки брошенные в момент получения токена они же отловленные в GlobalAsax не могут формировать Responce с HttpStatusCode.Unauthorized
                // в Responce приходит стандартный html вместо нашего текста ошибки
                // Чтоыбы отображать наш текст ошибки, отловленной в GlobalAsax, возвращаю со статусом OK
                httpContext.Response.StatusCode = fromGlobalAsax ? (int)HttpStatusCode.OK : (int)statusCode;


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

                // TODO - USER
                errorMessage += $"User: {user}\r\n";
                errorMessage += $"URL: {url}\r\n";
                errorMessage += $"Request Body: {body}\r\n";
                errorMessage += logExpression;



                // Этот иф мне не понятен. Почему StackTrace нужно пытаться брать из InnerException
                if (exception.InnerException != null)
                    exc = exception.InnerException;
                else
                    exc = exception;

                //errorMessage += $"StackTrace:\r\n{exc.StackTrace}\r\n";

                errorMessage += $"Request:\r\n{request}\r\n";

                

                FileLogger.AppendTextToFile(errorMessage, HttpContext.Current.Server.MapPath("~/SiteErrors.txt"));
            }
            catch { }
            #endregion log to file


            // Если возникли ошибки не совместивмые с дальнейшей работой пользователя - удаляю пользовательский контекст
            if (statusCode == HttpStatusCode.Unauthorized)
                DmsResolver.Current.Get<Utilities.UserContexts>().Remove();

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

    }
}