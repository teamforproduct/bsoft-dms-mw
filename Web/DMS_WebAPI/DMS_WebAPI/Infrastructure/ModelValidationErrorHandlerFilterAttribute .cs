using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DMS_WebAPI.Infrastructure
{
    [AttributeUsage(AttributeTargets.All)]
    public class ModelValidationErrorHandlerFilterAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                throw new IncomingModelIsNotValid();
            }
        }
    }
}
