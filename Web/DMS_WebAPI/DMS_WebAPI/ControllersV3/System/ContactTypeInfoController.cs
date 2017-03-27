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
using BL.Model.Common;
using System.Diagnostics;
using BL.Model.SystemCore;
using BL.Model.FullTextSearch;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Типы контактов
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.ContactType)]
    public class ContactTypeInfoController : ApiController
    {
        /// <summary>
        /// Возвращает список типов контактов
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontDictionaryContactType>))]
        public IHttpActionResult Get([FromUri] FullTextSearch ftSearch, [FromUri] FilterDictionaryContactType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetMainDictionaryContactTypes(ctx, ftSearch, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает тип контакта по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryContactType))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryContactType(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Создает новый тип контакта
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddContactType model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddContactType, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует тип контакта
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyContactType model)
        {
            Action.Execute(EnumDictionaryActions.ModifyContactType, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет тип контакта
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteContactType, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }
    }
}