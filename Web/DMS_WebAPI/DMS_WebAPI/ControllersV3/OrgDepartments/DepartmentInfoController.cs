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

namespace DMS_WebAPI.ControllersV3.OrgDepartments
{
    /// <summary>
    /// Отделы (подразделения) в органицации.
    /// Отдел всегда подчинен организации, может подчиняться вышестоящему отделу.
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Department)]
    public class DepartmentInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список отделов. 
        /// Отделы могут подчиняться вышестоящим отделам и всегда подчинены организации 
        /// </summary>
        /// <param name="filter">"</param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("Info")]
        //[ResponseType(typeof(List<FrontDictionaryDepartment>))]
        //public IHttpActionResult Get([FromUri] FilterDictionaryDepartment filter)
        //{
        //    if (!stopWatch.IsRunning) stopWatch.Restart();
        //    var ctx = DmsResolver.Current.Get<UserContexts>().Get();
        //    var tmpService = DmsResolver.Current.Get<IDictionaryService>();
        //    var tmpItems = tmpService.GetDictionaryDepartments(ctx, filter);
        //    var res = new JsonResult(tmpItems, this);
        //    res.SpentTime = stopWatch;
        //    return res;
        //}

        /// <summary>
        /// Возвращает отдел по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryDepartment))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryDepartment(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет отдел
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Info")]
        public IHttpActionResult Post([FromBody]AddDepartment model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDictionaryActions.AddDepartment, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует реквизиты отдела
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info")]
        public IHttpActionResult Put([FromBody]ModifyDepartment model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDictionaryActions.ModifyDepartment, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет отдел
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Info/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDictionaryActions.DeleteDepartment, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
