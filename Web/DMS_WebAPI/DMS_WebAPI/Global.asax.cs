using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Exception;
using DMS_WebAPI.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DMS_WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string libname);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FreeLibrary(IntPtr hModule);

        private static IntPtr LoadLibraryHandle { get; set; }

        protected void Application_Start()
        {
            // maximum number of concurrent connections allowed by a ServicePoint object
            System.Net.ServicePointManager.DefaultConnectionLimit = System.Int16.MaxValue;

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if (IntPtr.Size == 4)
            {
                // 32-bit
                LoadLibraryHandle = LoadLibrary(Path.Combine(HttpContext.Current.Server.MapPath("~/"), "App_Data", "CryptoExts", "x86", "CryptoExts.dll"));
            }
            else if (IntPtr.Size == 8)
            {
                // 64-bit
                LoadLibraryHandle = LoadLibrary(Path.Combine(HttpContext.Current.Server.MapPath("~/"), "App_Data", "CryptoExts", "x64", "CryptoExts.dll"));
            }


        }
        protected void Application_End(object sender, EventArgs e)
        {
            if (LoadLibraryHandle != IntPtr.Zero)
                FreeLibrary(LoadLibraryHandle);
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();

            // Handle HTTP errors
            //if (exc.GetType() == typeof(HttpException))
            //{
            //    // The Complete Error Handling Example generates
            //    // some errors using URLs with "NoCatch" in them;
            //    // ignore these here to simulate what would happen
            //    // if a global.asax handler were not implemented.
            //    if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
            //        return;

            //    //Redirect HTTP errors to HttpError page
            //    Server.Transfer("HttpErrorPage.aspx");
            //}

            // For other kinds of errors give the user some information
            // but stay on the default page
            //Response.Write("<h2>Global Page Error</h2>\n");
            //Response.Write(
            //    "<p>" + exc.Message + "</p>\n");
            //Response.Write("Return to the <a href='Default.aspx'>" +
            //    "Default Page</a>\n");

            var httpContext = HttpContext.Current;
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            httpContext.Response.ContentType = "application/json";

            //TODO Remove
            if (httpContext.IsDebuggingEnabled)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            if (exc is DmsExceptions)
            {
                if (exc is UserUnauthorized)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }


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


            httpContext.Response.Write(json);
            httpContext.Response.End();

            #region log to file
            try
            {
                var ex = exc;
                var errInfo = string.Empty;

                while (ex != null)
                {
                    var m = ex.Message;

                    if (!m.Contains("See the inner exception for details"))
                    {
                        errInfo += (errInfo == string.Empty ? "Exception:" : "InnerException:") + "\r\n";
                        errInfo += $"   Message: {ex.Message}\r\n";
                        errInfo += $"   Source: {ex.Source}\r\n";
                        errInfo += $"   Method: {ex.TargetSite}\r\n";

                    }
                    ex = ex.InnerException;
                };

                if (exc.InnerException != null) exc = exc.InnerException;

                errInfo += $"StackTrace:\r\n{exc.StackTrace}\r\n";


                // stores the error message
                string errorMessage = string.Empty;
                errorMessage += "ERROR!!! - " + DateTime.UtcNow.ToString("o") + "\r\n";

                try
                {
                    HttpContext cnt = httpContext;
                    errorMessage += $"URL: {cnt.Request.Url.ToString()}\r\n";
                }
                catch
                { }

                //errorMessage += $"Message:{ex.Message}\r\n";
                //errorMessage += $"Source:{ex.Source}\r\n";
                //errorMessage += $"Method:{ex.TargetSite}\r\n";
                //errorMessage += $"StackTrace:{ex.StackTrace}\r\n";
                errorMessage += errInfo;
                errorMessage += $"Request:\r\n{httpContext.Request}\r\n";

                try
                {
                    httpContext.Request.InputStream.Position = 0;
                    errorMessage += $"Request Body: {new System.IO.StreamReader(httpContext.Request.InputStream).ReadToEnd()}\r\n";
                }
                catch { }


                AppendToFile(httpContext.Server.MapPath("~/SiteErrors.txt"), errorMessage);
            }
            catch { }
            #endregion log to file

            //// Log the exception and notify system operators
            //ExceptionUtility.LogException(exc, "DefaultPage");
            //ExceptionUtility.NotifySystemOps(exc);

            // Clear the error from the server
            Server.ClearError();
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
            var languageService = DmsResolver.Current.Get<ILanguages>();
            return languageService.GetTranslation(text);
        }

        private void AppendToFile(string path, string text)
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
