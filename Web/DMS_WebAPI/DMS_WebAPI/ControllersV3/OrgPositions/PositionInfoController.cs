using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;

using BL.Model.Common;
using System.Web;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.DictionaryCore.FrontMainModel;
using System.Diagnostics;

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Должности.
    /// Должности всегда подчинены отделам.
    /// Значимость должносьти в отделе задается параметром Order
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Position)]
    public class PositionInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список должностей. 
        /// </summary>
        /// <param name="filter">"</param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("Info")]
        //[ResponseType(typeof(List<FrontDictionaryPosition>))]
        //public IHttpActionResult Get([FromUri] FilterDictionaryPosition filter)
        //{
        //    if (!stopWatch.IsRunning) stopWatch.Restart();
        //    var ctx = DmsResolver.Current.Get<UserContexts>().Get();
        //    var tmpService = DmsResolver.Current.Get<IDictionaryService>();
        //    var tmpItems = tmpService.GetDictionaryPositions(ctx, filter);
        //    var res = new JsonResult(tmpItems, this);
        //    res.SpentTime = stopWatch;
        //    return res;
        //}

        /// <summary>
        /// Возвращает должность по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryPosition))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryPosition(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет должность
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Info")]
        public IHttpActionResult Post([FromBody]AddDepartment model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDictionaryActions.AddPosition, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Изменяет порядок следования должности в отделе (нумерация с 1)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info/Order")]
        public IHttpActionResult SetOrder([FromBody]ModifyPositionOrder model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpItem = DmsResolver.Current.Get<IDictionaryService>();
            tmpItem.SetPositionOrder(cxt, model);
            var res = new JsonResult(model.Order, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Корректирует реквизиты должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info")]
        public IHttpActionResult Put([FromBody]ModifyDepartment model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDictionaryActions.ModifyPosition, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет должность
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Info/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDictionaryActions.DeletePosition, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
