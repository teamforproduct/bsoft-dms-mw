using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryAgentsController : ApiController
    {
        /// <summary>
        /// Получение словаря агентов
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns>Список агентов</returns>
        public IHttpActionResult Get([FromUri] FilterDictionaryAgent filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgents(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Получение словаря агентов по ИД
        /// </summary>
        /// <param name="id">ИД агента</param>
        /// <returns>Агент</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgent(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}