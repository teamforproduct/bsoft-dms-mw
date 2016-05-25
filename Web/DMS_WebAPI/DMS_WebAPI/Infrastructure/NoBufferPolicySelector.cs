using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.WebHost;

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
                    var listControllerNameUploadings = new List<string> { "DocumentFilesController" };
                    var controller = context.Request.RequestContext.RouteData.Values["controller"].ToString();

                    if (listControllerNameUploadings.Any(x=>x.Equals(controller,StringComparison.InvariantCultureIgnoreCase)))
                        return false;
                }
                catch(Exception ex)
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