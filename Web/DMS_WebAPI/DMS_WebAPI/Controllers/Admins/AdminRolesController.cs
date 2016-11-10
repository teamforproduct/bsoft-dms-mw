﻿using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Diagnostics;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Описание ролей. Роли объединяют наборы действий: работа с документами, визирование, работа с бумагой, настройка справочников...  
    /// Предустановленный роли имеют классификатор, для идентификации.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/AdminRoles")]
    public class AdminRolesController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает списоок ролей
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [ResponseType(typeof(List<FrontAdminRole>))]
        public IHttpActionResult Get([FromUri] FilterAdminRole filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAdminRoles(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает роль
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminRole</returns>
        [ResponseType(typeof(FrontAdminRole))]
        public IHttpActionResult Get(int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetAdminRoles(ctx, new FilterAdminRole() { IDs = new List<int> { id } });
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет новую роль. Роли нужно назначить действия AdminRoleActions.
        /// </summary>
        /// <param name="model">ModifyAdminRole</param>
        /// <returns>FrontAdminRole</returns>
        public IHttpActionResult Post([FromBody]ModifyAdminRole model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddRole, cxt, model);
            return Get((int)tmpItem);
        }

        /// <summary>
        /// Изменяет роль
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <param name="model">ModifyAdminRole</param>
        /// <returns>FrontAdminRole</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyAdminRole model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            tmpService.ExecuteAction(EnumAdminActions.ModifyRole, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет роль, отвязывает действия. Сами действия НЕ удаляются.
        /// </summary>
        /// <returns>FrontAdminRole</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();

            tmpService.ExecuteAction(EnumAdminActions.DeleteRole, cxt, id);
            var tmpItem = new FrontAdminRole() { Id = id };
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}