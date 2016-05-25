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
                var ListControllerNameUploadings = new List<string> { "DocumentFilesController" };
                if (string.Equals(context.Request.RequestContext.RouteData.Values["controller"].ToString(), "uploading", StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            return true;
        }

        public override bool UseBufferedOutputStream(HttpResponseMessage response)
        {
            return base.UseBufferedOutputStream(response);
        }
    }
}