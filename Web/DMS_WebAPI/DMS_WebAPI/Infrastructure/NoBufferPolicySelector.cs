using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Routing;

namespace DMS_WebAPI.Infrastructure
{
    public class NoBufferPolicySelector : WebHostBufferPolicySelector
    {
        public override bool UseBufferedInputStream(object hostContext)
        {
            var context = hostContext as HttpContextBase;

            if (context != null)
            {
                try
                {
                    var listControllerNameUploadings = new List<string> { "DocumentFiles" };

                    var routeDefaultApi = RouteTable.Routes["DefaultApi"] as Route;

                    var routeData = routeDefaultApi.GetRouteData(context);

                    var controller = routeData.Values["controller"].ToString();

                    if (listControllerNameUploadings.Any(x => x.Equals(controller, StringComparison.InvariantCultureIgnoreCase)))
                        return false;
                }
                catch (Exception ex)
                {

                }
            }

            return true;
        }

        public override bool UseBufferedOutputStream(HttpResponseMessage response)
        {
            return base.UseBufferedOutputStream(response);
        }
    }
}