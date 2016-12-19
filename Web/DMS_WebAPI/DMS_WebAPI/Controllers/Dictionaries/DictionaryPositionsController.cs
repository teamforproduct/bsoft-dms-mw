using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using System.Web.Http.Description;
using BL.Model.Common;
using System.Diagnostics;

namespace DMS_WebAPI.Controllers.Dictionaries
{

    /// <summary>
    /// Описывает должности в отделах.
    /// Должности всегда подчинены отделам.
    /// Значимость должносьти в отделе задается параметром Order
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DictionaryPositions")]
    public class DictionaryPositionsController : ApiController
    {

        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает записи из словаря "Должности"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Должности"</param>
        /// <returns>FrontDictionaryPositions</returns>
        [ResponseType(typeof(List<FrontDictionaryPosition>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryPosition filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositions(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Список (Id, Name) всех сотрудников
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetList")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryPosition filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetPositionList(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Возвращает запись из словаря "Должности" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryPositions</returns>
        [ResponseType(typeof(FrontDictionaryPosition))]
        public IHttpActionResult Get(int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryPosition(ctx, id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Изменяет порядок следования должности в отделе (нумерация с 1)
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetPositionOrder")]
        public IHttpActionResult SetPositionOrder([FromUri] int positionId, [FromUri] int order)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            tmpItem.SetPositionOrder(cxt, new ModifyPositionOrder { PositionId = positionId, Order = order });
            var res = new JsonResult(order, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавление записи в словаре "Должности"
        /// </summary>
        /// <param name="model">ModifyDictionaryPosition</param>
        /// <returns>DictionaryPositions</returns>
        public IHttpActionResult Post([FromBody]AddPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            int Id = (int)tmpItem.ExecuteAction(EnumDictionaryActions.AddPosition, cxt, model);
            return Get(new FilterDictionaryPosition { IDs = new List<int> { Id } });
        }

        /// <summary>
        /// Изменение записи в словаре "Должности"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryPosition</param>
        /// <returns>DictionaryPositions</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyPosition model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryPosition
            if (!stopWatch.IsRunning) stopWatch.Restart();
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            tmpItem.ExecuteAction(EnumDictionaryActions.ModifyPosition, cxt, model);
            return Get(new FilterDictionaryPosition { IDs = new List<int> { model.Id } }); 
        }

        /// <summary>
        /// Удаление записи в словаре "Должности"
        /// </summary>
        /// <returns>DictionaryPositions</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();

            tmpItem.ExecuteAction(EnumDictionaryActions.DeletePosition, cxt, id);
            FrontDictionaryPosition tmp = new FrontDictionaryPosition();
            tmp.Id = id;

            var res = new JsonResult(tmp, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}