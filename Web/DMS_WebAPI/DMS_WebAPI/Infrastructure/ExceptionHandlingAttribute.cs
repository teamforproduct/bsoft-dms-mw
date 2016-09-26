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

            var exc = context.Exception;

            string msgExp = string.Empty;
            //#if DEBUG
            //pss Убрать в продакшине, пока для понимания вопроса во время разработки пусть отображается полная информация!!!
            while (exc != null)
            {
                var m = exc.Message;

                m = ReplaceLanguageLabel(currentContext, m);

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

            //#else
            //msgExp = exc.Message;
            //#endif

            var json = JsonConvert.SerializeObject(new { success = false, msg = msgExp }, GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);
            //json = ReplaceLanguageLabel(currentContext, json);


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

        private string ReplaceLanguageLabel(HttpContext Context, string Label)
        {
            string res = Label;

            try
            {
                IContext ctx = null;
                try
                {
                    ctx = DmsResolver.Current.Get<UserContext>().GetByLanguage();
                    if (Context.User.Identity.IsAuthenticated && ctx != null)
                    {
                        var service = DmsResolver.Current.Get<ILanguageService>();
                        //Перевод ошибки
                        res = service.ReplaceLanguageLabel(ctx, res);
                    }
                }
                catch { }
                var languageService = DmsResolver.Current.Get<Languages>();
                //Перевод ошибки на русский
                res = languageService.ReplaceLanguageLabel(Context.Request.UserLanguages?[0], res);
            }
            catch { }

            return res;
        }

        private string InsertValues(string Message, List<string> Paramenters)
        {
            try
            {
                //pss До война дракона еще далеко... написал case в лоб
                switch (Paramenters.Count)
                {
                    case 1:
                        return string.Format(Message, Paramenters[0]);

                    case 2:
                        return string.Format(Message, Paramenters[0], Paramenters[1]);

                    case 3:
                        return string.Format(Message, Paramenters[0], Paramenters[1], Paramenters[2]);

                    case 4:
                        return string.Format(Message, Paramenters[0], Paramenters[1], Paramenters[2], Paramenters[3]);

                    case 5:
                        return string.Format(Message, Paramenters[0], Paramenters[1], Paramenters[2], Paramenters[3], Paramenters[4]);

                    case 6:
                        return string.Format(Message, Paramenters[0], Paramenters[1], Paramenters[2], Paramenters[3], Paramenters[4], Paramenters[5]);

                    case 7:
                        return string.Format(Message, Paramenters[0], Paramenters[1], Paramenters[2], Paramenters[3], Paramenters[4], Paramenters[5], Paramenters[6]);

                    case 8:
                        return string.Format(Message, Paramenters[0], Paramenters[1], Paramenters[2], Paramenters[3], Paramenters[4], Paramenters[5], Paramenters[6], Paramenters[7]);

                    case 9:
                        return string.Format(Message, Paramenters[0], Paramenters[1], Paramenters[2], Paramenters[3], Paramenters[4], Paramenters[5], Paramenters[6], Paramenters[7], Paramenters[8]);

                    default:
                        return Message;
                }

            }
            catch
            { }
            return Message;
        }
    }
}
