using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;
using BL.Logic.AdminCore.Interfaces;
using System.Linq;
using BL.Model.Exception;
using BL.Database.DatabaseContext;
using System.Collections.Generic;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class DimanicAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public override bool AllowMultiple { get; } = false;
        private EnumAccessTypes? _operationRight;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationRight"></param>
        public DimanicAuthorizeAttribute(string operationRight = null)
        {
            _operationRight = null;

            if (!string.IsNullOrEmpty(operationRight))
            {
                switch (operationRight)
                {
                    case "C": _operationRight = EnumAccessTypes.C; break;
                    case "U": _operationRight = EnumAccessTypes.U; break;
                    case "D": _operationRight = EnumAccessTypes.D; break;
                    case "R": _operationRight = EnumAccessTypes.R; break;
                }
            }
        }

        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            var principal = actionContext.RequestContext.Principal;
            if (principal == null)
            {
                //var controller = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
                //var method = actionContext.ActionDescriptor.ActionName;
                return Task.FromResult<HttpResponseMessage>(actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized));
            }

            var uri = actionContext.Request.RequestUri.AbsolutePath;

            if (!uri.StartsWith("/api/v3/")) Task.FromResult<HttpResponseMessage>(actionContext.Request.CreateResponse(HttpStatusCode.Forbidden));

            var param =uri.Replace("/api/v3/", "").Split('/');

            if (param.Length <2) Task.FromResult<HttpResponseMessage>(actionContext.Request.CreateResponse(HttpStatusCode.Forbidden));

            string module = param[0];
            string feature;
            int valueParsed;
            if (Int32.TryParse(param[1], out valueParsed) && param.Length >= 3)
            {
                feature = param[2];
            }
            else
            {
                feature = param[1];
            }

            if (_operationRight == null)
            {
                switch (actionContext.Request.Method.Method)
                {
                    case "POST": _operationRight = EnumAccessTypes.C; break;
                    case "PUT": _operationRight = EnumAccessTypes.U; break;
                    case "GET": _operationRight = EnumAccessTypes.R; break;
                    case "DELETE": _operationRight = EnumAccessTypes.D; break;
                }
            }

            if (CheckRight(module, feature, _operationRight.Value))
            {
                return continuation();
            }

            throw new AccessIsDenied();//pss так ничего не возвращается на фронт Task.FromResult<HttpResponseMessage>(actionContext.Request.CreateResponse(HttpStatusCode.NotAcceptable));
        }


        private bool CheckRight(string module, string feature, EnumAccessTypes right)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var permissionId = DmsDbImportData.GetPermissionId(module, feature, right);
            var res = tmpService.ExistsPermissionsAccess(ctx, tmpService.GetFilterPermissionsAccessByContext(ctx, false, new List<int> { permissionId }, null));
            // это не оптимально
            //var tmpItems = tmpService.GetUserPermissions(ctx);

            //// может сравнение без учета регистра
            //var res = tmpItems.Any(x =>
            //module.Equals(x.Module, StringComparison.OrdinalIgnoreCase)
            //&& feature.Equals(x.Feature, StringComparison.OrdinalIgnoreCase)
            //&& x.AccessType == right.ToString());

            return res;
        }

    }
}