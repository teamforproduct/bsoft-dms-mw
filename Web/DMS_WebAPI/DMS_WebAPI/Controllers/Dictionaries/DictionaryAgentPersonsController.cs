using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.SystemCore;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    [Authorize]
    public class DictionaryAgentPersonsController : ApiController
    {
       
        /// <summary>
        /// ПОлучить список физлиц
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns>коллекцию контрагентов</returns>
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentPerson filter, [FromUri]UIPaging paging)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentPersons(cxt, filter,paging);
            return new JsonResult(tmpDicts, this);
        }

       /// <summary>
       /// Получить физлицо по ИД
       /// </summary>
       /// <param name="id">ИД</param>
       /// <returns>запись справочника агентов-физлиц</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentPerson(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// добавить физлицо
        /// </summary>
        /// <param name="model">параметры физлица</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryAgentPerson model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentPerson, cxt, model));
        }

        /// <summary>
        /// добавить реквизиты физлица к существующему контрагенту
        /// </summary>
        /// <param name="AgentId">ИД агента</param>
        /// <param name="model">параметры физлица</param>
        /// <returns>добавленную запись</returns>
        public IHttpActionResult PostToExistingAgent(int AgentId,[FromBody]ModifyDictionaryAgentPerson model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            model.Id = AgentId;
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentPerson, cxt, model));
        }
        /// <summary>
        /// изменить физлицо. контрагент меняется, если он является только физлицом
        /// </summary>
        /// <param name="id">ИД</param>
        /// <param name="model">параметры</param>
        /// <returns>возвращает измененную запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgentPerson model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentPerson, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// удалить физлицо, контрагент удаляется,  если он является только физлицом
        /// </summary>
        /// <param name="id">ИД</param>
        /// <returns>ИД удаленной записи</returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentPerson, cxt, id);
            
             
            FrontDictionaryAgentPerson tmp = new FrontDictionaryAgentPerson();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }

    }
}