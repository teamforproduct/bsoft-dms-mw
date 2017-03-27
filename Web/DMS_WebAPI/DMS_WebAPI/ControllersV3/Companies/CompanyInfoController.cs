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

namespace DMS_WebAPI.ControllersV3.Companies
{
    /// <summary>
    /// Юридические лица
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Company)]
    public class CompanyInfoController : ApiController
    {
        /// <summary>
        /// Список юридических лиц
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <param name="sorting">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontMainAgentCompany>))]
        public IHttpActionResult GetMain([FromUri]FullTextSearch ftSearch, [FromUri] FilterDictionaryAgentCompany filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetMainAgentCompanies(ctx, ftSearch, filter, paging, sorting);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            return res;
        }


        /// <summary>
        /// Возвращает реквизиты юридического лица
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontAgentCompany))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentCompany(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Добавляет юридическое лицо
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddAgentCompany model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddAgentCompany, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует реквизиты юридического лица
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyAgentCompany model)
        {
            Action.Execute(EnumDictionaryActions.ModifyAgentCompany, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет юридическое лицо
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteAgentCompany, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
