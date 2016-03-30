using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryRegistrationJournalsController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Журналы регистрации"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Журналы регистрации"</param>
        /// <returns>FrontDictionaryRegistrationJournals</returns>
        // GET: api/DictionaryRegistrationJournals
        public IHttpActionResult Get([FromUri] FilterDictionaryRegistrationJournal filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryRegistrationJournals(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Журналы регистрации" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryRegistrationJournals</returns>
        // GET: api/DictionaryRegistrationJournals/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryRegistrationJournal(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление записи в словаре "Журналы регистрации"
        /// </summary>
        /// <param name="model">ModifyDictionaryRegistrationJournal</param>
        /// <returns>FrontDictionaryRegistrationJournals</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryRegistrationJournal model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddRegistrationJournal, cxt, model));
        }

        /// <summary>
        /// Изменение записи в словаре "Журналы регистрации"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryRegistrationJournal</param>
        /// <returns>FrontDictionaryRegistrationJournals</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryRegistrationJournal model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryRegistrationJournal

            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyRegistrationJournal, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Журналы регистрации"
        /// </summary>
        /// <returns>FrontDictionaryRegistrationJournal</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteRegistrationJournal, cxt, id);
            FrontDictionaryRegistrationJournal tmp = new FrontDictionaryRegistrationJournal();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
       
    }
}