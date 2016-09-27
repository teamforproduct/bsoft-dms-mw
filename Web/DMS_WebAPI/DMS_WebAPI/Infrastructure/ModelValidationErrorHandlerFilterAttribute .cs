using BL.Model.Exception;
using DMS_WebAPI.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DMS_WebAPI.Infrastructure
{
    [AttributeUsage(AttributeTargets.All)]
    public class ModelValidationErrorHandlerFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                string res = null;
                // Добавляю информацию: "Почему модель не валидна"
                foreach (var item in actionContext.ModelState.Values)
                {
                    foreach (var er in item.Errors)
                    {
                        string msg = (er.ErrorMessage == string.Empty) ? er.Exception?.Message : er.ErrorMessage;
                        if (msg != string.Empty) res = msg + "; " + res ;
                    }
                }

                //var json = JsonConvert.SerializeObject(actionContext.ModelState.Values, GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings);

                throw new IncomingModelIsNotValid(res);
            }
        }
    }
}
