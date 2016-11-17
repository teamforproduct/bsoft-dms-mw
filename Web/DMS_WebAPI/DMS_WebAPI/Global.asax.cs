using System;
using System.IO;
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
    }
}
