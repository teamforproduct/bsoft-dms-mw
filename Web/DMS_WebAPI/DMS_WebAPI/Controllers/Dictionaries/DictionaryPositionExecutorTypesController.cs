using BL.CrossCutting.DependencyInjection;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryPositionExecutorTypesController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Типы исполнителей"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Типы исполнителей"</param>
        /// <returns>FrontDictionaryPositionExecutorTypes</returns>
        // GET: api/DictionaryPositionExecutorType
        public IHttpActionResult Get([FromUri] FilterDictionaryPositionExecutorType filter)
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryPositionExecutorTypes(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Типы исполнителей" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryPositionExecutorTypes</returns>
        // GET: api/DictionaryPositionExecutorType/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryPositionExecutorType(cxt, id);
            return new JsonResult(tmpDict, this);
        }

    }
}