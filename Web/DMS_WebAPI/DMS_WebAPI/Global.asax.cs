using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;

namespace DMS_WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //TODO: Remove this when user auth will be implemented. Fill context from session
            var ctx = DmsResolver.Current.Get<IContext>();
            ctx.CurrentDB.Address = @"..\..\..\..\BusinessLayer\BL.Database\DBFile\DmsLocalDB.mdf";
            ctx.CurrentDB.IntegrateSecurity = true;
            ctx.CurrentDB.ServerType = DatabaseType.SQLPortable;
        }
    }
}
