using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Exception;
using DMS_WebAPI.Infrastructure;
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
            BL.CrossCutting.Helpers.FileLogger.AppendTextToSiteErrors("Application_End");

            if (LoadLibraryHandle != IntPtr.Zero)
                FreeLibrary(LoadLibraryHandle);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();

            // Игнорируем ошибки, которые пока не умеем выключить
            // Server cannot set status after HTTP headers have been sent.
            if (exc is System.Web.HttpException)
            {
                if ((exc as System.Web.HttpException).ErrorCode == -2147467259) return;
            }

            ExceptionHandling.ReturnExceptionResponse(exc);

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



            //// Log the exception and notify system operators
            //ExceptionUtility.LogException(exc, "DefaultPage");
            //ExceptionUtility.NotifySystemOps(exc);

            // Clear the error from the server
            Server.ClearError();
        }

        
    }
}
