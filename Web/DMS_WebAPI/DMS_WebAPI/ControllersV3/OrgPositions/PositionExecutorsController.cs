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
using BL.Model.Common;
using System.Diagnostics;

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Исполнители должности.
    /// Обязанности должности (роли) может выполнять сотрудник назначенный на должность, временно исполняющий обязанности или референт.
    /// Исполнители назначаются на определенный срок.
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Position)]
    public class PositionExecutorsController : ApiController
    {

        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список назначений
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/Executors")]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterDictionaryPositionExecutor();
            filter.PositionIDs = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает список текущих (актуальных) назначений
        /// </summary>
        /// <param name="PositionId">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Executors/Current")]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public IHttpActionResult GetCurrent(int PositionId, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            filter.PositionIDs = new List<int> { PositionId };
            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;


            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает назначение по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Executors/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryPositionExecutor))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryPositionExecutor(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Назначает сотрудника исполнителем обязанностей должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Executors")]
        public IHttpActionResult Post([FromBody]AddPositionExecutor model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDictionaryActions.AddEmployeeAddress, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует параметры назначения
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Executors")]
        public IHttpActionResult Put([FromBody]ModifyPositionExecutor model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDictionaryActions.ModifyEmployeeAddress, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет назначение
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Executors/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDictionaryActions.DeleteEmployeeAddress, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;

        }
    }
}