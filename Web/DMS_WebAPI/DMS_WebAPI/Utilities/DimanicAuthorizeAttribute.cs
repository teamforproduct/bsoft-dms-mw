using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace DMS_WebAPI.Utilities
{
    public class DimanicAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public bool AllowMultiple {
            get { return false; }
        }

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            var principal = actionContext.RequestContext.Principal;
            if (principal == null)
            {
                return Task.FromResult<HttpResponseMessage>(actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized));
            }
            else
            {
                return continuation();
            }
        }

        public void OnAuthorization(System.Web.Http.Controllers.HttpActionContext filterContext)
        {
            string action = filterContext.ActionDescriptor.ActionName;
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            //string user = filterContext.HttpContext.User.Identity.Name;
            var i = 1;
        }
    }
}