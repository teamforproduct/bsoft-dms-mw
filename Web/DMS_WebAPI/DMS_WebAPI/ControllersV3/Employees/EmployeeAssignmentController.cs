using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Employees
{
    /// <summary>
    /// Сотрудники. Назначения сотрудника на должности
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Employee)]
    public class EmployeeAssignmentsController : ApiController
    {
        /// <summary>
        /// Возвращает список назначений сотрудника (история назначений)
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Assignments)]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();
            filter.AgentIDs = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает список назначений сотрудника (только текущие, актуальные назначения)
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Assignments + "/Current")]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public IHttpActionResult GetCurrent(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            filter.AgentIDs = new List<int> { Id };
            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;
            filter.IsActive = true;


            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает список ролей должности, на которую назначается сотрудник
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Assignments + "/Position/{Id:int}/Roles")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetPositionRoles(int Id, [FromUri] FilterAdminRole filter, [FromUri]UIPaging paging)
        {
            if (filter == null) filter = new FilterAdminRole();

            filter.PositionIDs = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetListRoles(ctx,  filter, paging);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает назначение по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Assignments + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryPositionExecutor))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryPositionExecutor(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Назначает сотрудника на новую должность (при назначении сотруднику устанавлючаются все роли от должности)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Assignments)]
        public IHttpActionResult Post([FromBody]AddPositionExecutor model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddExecutor, ctx, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует параметры назначения сотрудника на должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Assignments)]
        public IHttpActionResult Put([FromBody]ModifyPositionExecutor model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.ModifyExecutor, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет назначение сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Assignments + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            tmpService.ExecuteAction(EnumDictionaryActions.DeleteExecutor, ctx, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;

        }
    }
}