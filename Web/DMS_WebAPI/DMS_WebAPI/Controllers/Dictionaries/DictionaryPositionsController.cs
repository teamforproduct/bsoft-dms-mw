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

namespace DMS_WebAPI.Controllers.Dictionaries
{

    /// <summary>
    /// Описывает должности в отделах.
    /// Должности всегда подчинены отделам.
    /// Значимость должносьти в отделе задается параметром Order
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/DictionaryPositions")]
    public class DictionaryPositionsController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Должности"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Должности"</param>
        /// <returns>FrontDictionaryPositions</returns>
        [ResponseType(typeof(List<FrontDictionaryPosition>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryPosition filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryPositions(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Должности" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryPositions</returns>
        [ResponseType(typeof(FrontDictionaryPosition))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryPositions(ctx, new FilterDictionaryPosition { IDs = new List<int> { id } });
            return new JsonResult(tmpItem, this);
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            tmpItem.SetPositionOrder(cxt, positionId, order);

            return new JsonResult(order, this);
        }

        /// <summary>
        /// Добавление записи в словаре "Должности"
        /// </summary>
        /// <param name="model">ModifyDictionaryPosition</param>
        /// <returns>DictionaryPositions</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryPosition model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpItem.ExecuteAction(EnumDictionaryActions.AddPosition, cxt, model));
        }

        /// <summary>
        /// Изменение записи в словаре "Должности"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryPosition</param>
        /// <returns>DictionaryPositions</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryPosition model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryPosition

            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            tmpItem.ExecuteAction(EnumDictionaryActions.ModifyPosition, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Должности"
        /// </summary>
        /// <returns>DictionaryPositions</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();

            tmpItem.ExecuteAction(EnumDictionaryActions.DeletePosition, cxt, id);
            FrontDictionaryPosition tmp = new FrontDictionaryPosition();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}