using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Persons
{
    /// <summary>
    /// Физические лица
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Person)]
    public class PersonInfoController : ApiController
    {
        /// <summary>
        /// Список физических лиц
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontMainAgentPerson>))]
        public IHttpActionResult GetMain([FromUri]FullTextSearch ftSearch, [FromUri] FilterDictionaryAgentPerson filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetMainAgentPersons(ctx, ftSearch, filter, paging, sorting);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            return res;
        }


        /// <summary>
        /// Возвращает реквизиты физического лица
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontMainAgentPerson))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentPerson(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Добавляет физическое лицо
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddAgentPerson model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddAgentPerson, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует реквизиты физического лица
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyAgentPerson model)
        {
            Action.Execute(EnumDictionaryActions.ModifyAgentPerson, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет физическое лицо
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteAgentPerson, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
