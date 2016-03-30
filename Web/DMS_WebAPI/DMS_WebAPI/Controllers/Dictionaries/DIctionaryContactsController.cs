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
    /// Список контактов контрагента
    /// </summary>
    [Authorize]
    public class DictionaryContactsController : ApiController
    {

        /// <summary>
        /// Возвращает список контактов
        /// </summary>
        /// <param name="agentId">ID контрагента</param>
        /// <param name="filter">Параметры для фильтрации контактов</param>
        /// <returns>Cписок контактов контрагента</returns>
        // GET: api/DictionaryContacts
        public IHttpActionResult Get(int agentId,[FromUri] FilterDictionaryContact filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryContacts(cxt, agentId, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает контакт
        /// </summary>
        // GET: api/DictionaryAddressTypes/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryContact(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление записи в справочник контактов
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный запись словаря типа документа</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryContact model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddContact, cxt, model));
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyContact, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет из справочника запись
        /// </summary>
        /// <returns>Возвращает id удаленной записи</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteContact, cxt, id);
            FrontDictionaryContact tmp = new FrontDictionaryContact();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}
