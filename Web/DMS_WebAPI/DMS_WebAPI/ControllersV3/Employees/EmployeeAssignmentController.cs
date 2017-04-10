using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
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
using System.Threading.Tasks;
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
    public class EmployeeAssignmentsController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryPositionExecutor(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список назначений сотрудника (история назначений)
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Assignments)]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();
            filter.AgentIDs = new List<int> { Id };

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(context, filter);
            var res = new JsonResult(tmpItems, this);
            return res;});
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
        public async Task<IHttpActionResult> GetCurrent(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            filter.AgentIDs = new List<int> { Id };
            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;
            filter.IsActive = true;


            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(context, filter);
            var res = new JsonResult(tmpItems, this);
            return res;});
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
        public async Task<IHttpActionResult> GetPositionRoles(int Id, [FromUri] FilterAdminRole filter, [FromUri]UIPaging paging)
        {
            if (filter == null) filter = new FilterAdminRole();

            filter.PositionIDs = new List<int> { Id };

return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetListRoles(context,   filter, paging);
            var res = new JsonResult(tmpItems, this);
            return res;});
        }

        /// <summary>
        /// Возвращает назначение по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Assignments + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryPositionExecutor))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Назначает сотрудника на новую должность (при назначении сотруднику устанавлючаются все роли от должности)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Assignments)]
        public async Task<IHttpActionResult> Post([FromBody]AddPositionExecutor model)
        {return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddExecutor, context,  model);
            return GetById(context, tmpItem);});
        }

        /// <summary>
        /// Корректирует параметры назначения сотрудника на должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Assignments)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyPositionExecutor model)
        {return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.ModifyExecutor, context, model);
            return GetById(context, model.Id);});
        }

        /// <summary>
        /// Удаляет назначение сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Assignments + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {return await SafeExecuteAsync(ModelState, (context, param) =>
            {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            tmpService.ExecuteAction(EnumDictionaryActions.DeleteExecutor, context, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;});

        }
    }
}