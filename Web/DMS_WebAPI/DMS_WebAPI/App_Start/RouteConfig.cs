using System.Web.Mvc;
using System.Web.Routing;

namespace DMS_WebAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional});

            //routes.MapRoute(
            //    name: "GetFiles",
            //    url: "/api/v3/image/files/{clientId:int}/{fileType:int}/{fileId:int}",
            //    defaults: new {controller = "Image", action = "GetFile", clientId = UrlParameter.Optional, fileType = UrlParameter.Optional, fileId = UrlParameter.Optional});
        }
    }
}
