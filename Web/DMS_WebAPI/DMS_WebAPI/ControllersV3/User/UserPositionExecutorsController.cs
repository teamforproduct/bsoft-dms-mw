using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
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
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Делегирование полномочий. Пользователь может делегировать часть своих полномочий: назначить себе ио или референта
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserPositionExecutorsController : ApiController
    {
        /// <summary>
        /// Возвращает список исполнителей должности (только текущие, актуальные назначения)
        /// Только должности, к которые исполняет текущий пользователь
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Positions/{Id:int}/" + Features.Executors)]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetUserPositionExecutors(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }


        /// <summary>
        /// Возвращает назначение по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Positions/" + Features.Executors + " /{Id:int}")]
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
        /// Назначает ио или референта на должность пользователя (при делегировании полномочий делегату устанавлючаются все роли от сотрудника-пользователя по должности)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Positions/" + Features.Executors)]
        public IHttpActionResult Post([FromBody]AddPositionExecutor model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddUserPositionExecutor, ctx, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует параметры назначения сотрудника на должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Positions/" + Features.Executors)]
        public IHttpActionResult Put([FromBody]ModifyPositionExecutor model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.ModifyUserPositionExecutor, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет назначение сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Positions/" + Features.Executors + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            tmpService.ExecuteAction(EnumDictionaryActions.DeleteUserPositionExecutor, ctx, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;

        }
    }
}