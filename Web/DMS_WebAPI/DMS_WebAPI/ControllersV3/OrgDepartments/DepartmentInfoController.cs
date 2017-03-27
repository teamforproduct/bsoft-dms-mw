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
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.OrgDepartments
{
    /// <summary>
    /// Отделы органицации.
    /// Отдел всегда подчинен организации, может подчиняться вышестоящему отделу.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Department)]
    public class DepartmentInfoController : ApiController
    {
        /// <summary>
        /// Возвращает список отделов. 
        /// Отделы могут подчиняться вышестоящим отделам и всегда подчинены организации 
        /// </summary>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info)]
        [ResponseType(typeof(List<FrontDictionaryDepartment>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryDepartment filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryDepartments(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает отдел по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryDepartment))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryDepartment(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Добавляет отдел
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddDepartment model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddDepartment, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует реквизиты отдела
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyDepartment model)
        {
            Action.Execute(EnumDictionaryActions.ModifyDepartment, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет отдел
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteDepartment, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
