using BL.Model.Exception;
using DMS_WebAPI.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace DMS_WebAPI.Infrastructure
{
    [AttributeUsage(AttributeTargets.All)]
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.OK;
            if (context.Exception is DmsExceptions)
            {
                if (context.Exception is UserUnauthorized)
                {
                    HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    
                }
            }
            else
            {

            }
            HttpContext.Current.Response.Write(new { success = false, msg = context.Exception.Message });
            HttpContext.Current.Response.End();
        }
    }
}
