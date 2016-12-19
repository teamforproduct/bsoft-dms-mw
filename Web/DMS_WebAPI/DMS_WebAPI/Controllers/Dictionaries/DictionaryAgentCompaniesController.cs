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
using BL.Model.Common;
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Dictionaries
{   /// <summary>
    /// Контрагенты - юридические лица
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DictionaryAgentCompanies")]
    public class DictionaryAgentCompaniesController : ApiController
    {
        /// <summary>
        /// Список контрагентов - юридических лиц
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        // GET: api/DictionaryCompanies
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentCompany filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetAgentCompanies(ctx, filter, paging);
            var res=new JsonResult(tmpDicts, this);
            res.Paging = paging;
            return res;
        }
        /// <summary>
        /// контрагент - юридическое лицо
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/DictionaryCompanies/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetAgentCompany(ctx, id);
            return new JsonResult(tmpDict, this);
        }


        /// <summary>
        /// Возвращает список (Id, Name) всех юридических лиц
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetList")]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult GetList([FromUri] FilterDictionaryAgentCompany filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentCompanyList(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.Paging = paging;
            return res;
        }


        /// <summary>
        /// добавить юридическое лицо
        /// </summary>
        /// <param name="model">параметры юрлица</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult Post([FromBody]AddAgentCompany model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentCompany, ctx, model));
        }

        /// <summary>
        /// добавить реквизиты юридического лица к существующему контрагенту
        /// </summary>
        /// <param name="AgentId">ИД агента</param>
        /// <param name="model">параметры юрлица</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult PostToExistingAgent(int AgentId, [FromBody]ModifyAgentCompany model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            model.Id = AgentId;
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentCompany, ctx, model));
        }
        /// <summary>
        /// изменить юридическое лицо
        /// </summary>
        /// <param name="id">ИД</param>
        /// <param name="model">параметры</param>
        /// <returns>возвращает измененную запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyAgentCompany model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentCompany, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// удалить юридическое лицо, контрагент удаляется,  если он является только юрлицом
        /// </summary>
        /// <param name="id">ИД</param>
        /// <returns>ИД удаленной записи</returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentCompany, ctx, id);
            FrontDictionaryAgentCompany tmp = new FrontDictionaryAgentCompany();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }

    }
}