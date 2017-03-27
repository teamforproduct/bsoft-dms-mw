using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

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
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryPositionExecutor(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }


        /// <summary>
        /// Возвращает список назначений
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Executors)]
        [ResponseType(typeof(List<FrontDictionaryPositionExecutor>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
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
        public async Task<IHttpActionResult> GetCurrent(int Id, [FromUri] FilterDictionaryPositionExecutor filter)
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
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Назначает сотрудника исполнителем обязанностей должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Executors)]
        public async Task<IHttpActionResult> Post([FromBody]AddPositionExecutor model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddExecutor, model);
            return GetById(context, tmpItem);
        }

        /// <summary>
        /// Корректирует параметры назначения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Executors)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyPositionExecutor model)
        {
            Action.Execute(EnumDictionaryActions.ModifyExecutor, model);
            return GetById(context, model.Id);
        }

        /// <summary>
        /// Удаляет назначение
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Executors + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteExecutor, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;

        }
    }
}