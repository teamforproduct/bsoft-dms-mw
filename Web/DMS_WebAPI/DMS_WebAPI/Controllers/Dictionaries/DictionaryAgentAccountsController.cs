using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{   
    /// <summary>
    /// Справочник расчетных счетов
    /// </summary>
    [Authorize]
    public class DictionaryAgentAccountsController : ApiController
    {


        /// <summary>
        /// Справочник расчетных счетов
        /// </summary>
        /// <param name="agentId">ID агента</param>
        /// <param name="filter">фильтр</param>
        /// <returns>Список расчетных счетов контрагента
        /// </returns>
        // GET: api/DictionaryAgentAccounts
        public IHttpActionResult Get(int agentId,[FromUri] FilterDictionaryAgentAccount filter)
        {
           

            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentAccounts(cxt, agentId,filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Получение расчетного счета по ИД
        /// </summary>
        /// <param name="id">ИД счета</param>
        /// <returns>Расчетный счет</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentAccount(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление расчетного счета
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryAgentAccount model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentAccount, cxt, model));
        }

        /// <summary>
        /// Изменение расчетного счета
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgent model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentAccount, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление расчетного счета
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentAccount, cxt, id);
            FrontDictionaryAgentAccount tmp = new FrontDictionaryAgentAccount();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}
