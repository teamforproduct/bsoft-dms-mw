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

namespace DMS_WebAPI.Controllers.Dictionaries

{
    /// <summary>
    /// Список контактов контрагента
    /// </summary>
    [Authorize]
    public class DictionaryAgentContactsController : ApiController
    {

        /// <summary>
        /// Возвращает список контактов
        /// </summary>
        /// <param name="agentId">ID контрагента</param>
        /// <param name="filter">Параметры для фильтрации контактов</param>
        /// <returns>Cписок контактов контрагента</returns>
        // GET: api/DictionaryContacts
        [ResponseType(typeof(List<FrontDictionaryContact>))]
        public IHttpActionResult Get(int agentId,[FromUri] FilterDictionaryContact filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();

            if (filter == null) filter = new FilterDictionaryContact();

            if (filter.AgentIDs == null)
                filter.AgentIDs = new List<int> { agentId };
            else
                filter.AgentIDs.Add(agentId);

            var tmpDicts = tmpDictProc.GetDictionaryContacts(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает контакт
        /// </summary>
        [ResponseType(typeof(FrontDictionaryContact))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryContact(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление записи в справочник контактов
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа документа</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryContact model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentContact, ctx, model));
        }

        /// <summary>
        /// Изменение записи справочника типы адресов 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа адреса</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryContact model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentContact, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет из справочника запись
        /// </summary>
        /// <returns>Возвращает id удаленной записи</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentContact, ctx, id);
            FrontDictionaryContact tmp = new FrontDictionaryContact();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}
