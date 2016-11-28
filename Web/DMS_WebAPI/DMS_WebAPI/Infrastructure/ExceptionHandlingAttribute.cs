using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DependencyInjection;
using BL.Model.Exception;
using DMS_WebAPI.Utilities;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using DMS_WebAPI.Models;

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
