using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DictionaryRegistrationJournals")]
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
            return new JsonResult(null, this);
        }

       
    }
}