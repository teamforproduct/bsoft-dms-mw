﻿using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Сессии 
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserSessionsController : WebApiController
    {

        /// <summary>
        /// Возвращает историю подключений
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Sessions)]
        [ResponseType(typeof(FrontSystemSession))]
        public async Task<IHttpActionResult> Get([FromUri]FilterSystemSession filter, [FromUri]UIPaging paging)
        {
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var sesions = ctxs.GetContextListQuery();

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var sessParam = (IQueryable<FrontSystemSession>) param;
                var tmpService = DmsResolver.Current.Get<ILogger>();
                if (filter == null) filter = new FilterSystemSession();
                filter.ExecutorAgentIDs = new List<int> { context.CurrentAgentId };
                var tmpItems = tmpService.GetSystemSessions(context, sessParam, filter, paging);
                var res = new JsonResult(tmpItems, this);
                return res;
            }, sesions);
        }

        /// <summary>
        /// Возвращает текущие подключения
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Sessions + "/Current")]
        [ResponseType(typeof(FrontSystemSession))]
        public async Task<IHttpActionResult> GetCurrent([FromUri]FilterSystemSession filter, [FromUri]UIPaging paging)
        {
            var ctxs = DmsResolver.Current.Get<UserContexts>();
            var sesions = ctxs.GetContextListQuery();

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ILogger>();
                if (filter == null) filter = new FilterSystemSession();
                filter.ExecutorAgentIDs = new List<int> { context.CurrentAgentId };
                filter.IsOnlyActive = true;
                var tmpItems = tmpService.GetSystemSessions(context, sesions, filter, paging);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }


    }
}
