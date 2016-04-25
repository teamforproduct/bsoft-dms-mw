using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Справочник результатов исполнения (не модифицируется пользователем)
    /// </summary>
    [Authorize]
    public class DictionaryResultTypesController : ApiController
    {
        /// <summary>
        /// Список типов результатов исполнения
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        // GET: api/DictionaryResultTypes
        public IHttpActionResult Get([FromUri] FilterDictionaryResultType filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryResultTypes(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }
        /// <summary>
        /// Получить результат исполнения по ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/DictionaryResultTypes/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryResultType(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}