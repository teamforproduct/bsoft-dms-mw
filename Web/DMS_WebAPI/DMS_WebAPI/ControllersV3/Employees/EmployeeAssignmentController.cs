using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;
using System;

namespace DMS_WebAPI.ControllersV3.Employees
{
    /// <summary>
    /// Назначения сотрудника на должности
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + "Employee")]
    public class EmployeeAssignmentController : ApiController
    {

        /// <summary>
        /// Возвращает список назначений сотрудника
        /// </summary>
        /// <param name="EmployeeId">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Assignments")]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public IHttpActionResult Get(int EmployeeId, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            filter.AgentIDs = new List<int> { EmployeeId };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает список назначений сотрудника
        /// </summary>
        /// <param name="EmployeeId">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Assignments/Current")]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public IHttpActionResult GetCurrent(int EmployeeId, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            filter.AgentIDs = new List<int> { EmployeeId };
            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;


            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает адрес по ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Assignments/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryPositionExecutor))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryPositionExecutor(ctx, Id);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Назначает сотрудника на новую должность
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Assignments")]
        public IHttpActionResult Post([FromBody]AddPositionExecutor model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpService.ExecuteAction(EnumDictionaryActions.AddEmployeeAddress, ctx, model));
        }

        /// <summary>
        /// Корректирует параметры назначения сотрудника на должности
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Assignments")]
        public IHttpActionResult Put([FromBody]ModifyPositionExecutor model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.ModifyEmployeeAddress, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет назначение сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Assignments/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            tmpService.ExecuteAction(EnumDictionaryActions.DeleteEmployeeAddress, ctx, Id);
            FrontDictionaryAgentAddress tmp = new FrontDictionaryAgentAddress();
            tmp.Id = Id;

            return new JsonResult(tmp, this);

        }
    }
}