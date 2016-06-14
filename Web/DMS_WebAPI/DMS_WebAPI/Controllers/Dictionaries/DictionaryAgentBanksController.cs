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

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Контрагенты - банки
    /// </summary>
    [Authorize]
    public class DictionaryAgentBanksController : ApiController
    {
        /// <summary>
        /// Список контрагентов - банков
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentBank filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentBanks(ctx, filter, paging);
            var res= new JsonResult(tmpDicts, this);
            res.Paging = paging;
            return res;
        }
        /// <summary>
        /// контрагент - банк
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/DictionaryCompanies/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentBank(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// добавить банк
        /// </summary>
        /// <param name="model">параметры юрлица</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryAgentBank model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentBank, ctx, model));
        }

        /// <summary>
        /// добавить реквизиты банка к существующему контрагенту
        /// </summary>
        /// <param name="AgentId">ИД агента</param>
        /// <param name="model">параметры банка</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult PostToExistingAgent(int AgentId, [FromBody]ModifyDictionaryAgentBank model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            model.Id = AgentId;
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentBank, ctx, model));
        }
        /// <summary>
        /// изменить юридическое лицо
        /// </summary>
        /// <param name="id">ИД</param>
        /// <param name="model">параметры</param>
        /// <returns>возвращает измененную запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgentBank model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentBank, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// удалить банк
        /// </summary>
        /// <param name="id">ИД</param>
        /// <returns>ИД удаленной записи</returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentCompany, ctx, id);
            FrontDictionaryAgentBank tmp = new FrontDictionaryAgentBank();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }

    }
}
