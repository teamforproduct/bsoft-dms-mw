using System;
using System.Web.Http.Filters;

namespace DMS_WebAPI.Infrastructure
{
    [AttributeUsage(AttributeTargets.All)]
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            ExceptionHandling.ReturnExceptionResponse(context.Exception, context);
        }
    }
}
