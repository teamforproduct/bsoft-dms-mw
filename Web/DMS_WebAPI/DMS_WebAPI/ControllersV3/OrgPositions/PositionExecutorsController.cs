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
using BL.Model.SystemCore;

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Должности. Исполнители.
    /// Обязанности должности (роли) может выполнять сотрудник назначенный на должность, временно исполняющий обязанности или референт.
    /// Исполнители назначаются на определенный срок.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Position)]
    public class PositionExecutorsController : ApiController
    {
        /// <summary>
        /// Возвращает список назначений
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Executors)]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();
            filter.PositionIDs = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает список текущих (актуальных) назначений
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Executors + "/Current")]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public IHttpActionResult GetCurrent(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            if (filter == null) filter = new FilterDictionaryPositionExecutor();

            filter.PositionIDs = new List<int> { Id };
            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;


            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositionExecutors(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает назначение по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Executors + "/{Id:int}")]
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
        /// Назначает сотрудника исполнителем обязанностей должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Executors)]
        public IHttpActionResult Post([FromBody]AddPositionExecutor model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddExecutor, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует параметры назначения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Executors)]
        public IHttpActionResult Put([FromBody]ModifyPositionExecutor model)
        {
            Action.Execute(EnumDictionaryActions.ModifyExecutor, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет назначение
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Executors + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteExecutor, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;

        }
    }
}