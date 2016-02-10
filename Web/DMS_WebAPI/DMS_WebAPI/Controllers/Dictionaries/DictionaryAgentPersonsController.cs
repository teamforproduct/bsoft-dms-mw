using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryAgentPersonsController : ApiController
    {
        /// <summary>
        /// Получение словаря контактов посторонней организации
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns>Список контактов посторонних организаций</returns>
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentPerson filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentPersons(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Получение словаря контактов посторонней организации по ИД
        /// </summary>
        /// <param name="id">ИД контакта</param>
        /// <returns>Контакт</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentPerson(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}