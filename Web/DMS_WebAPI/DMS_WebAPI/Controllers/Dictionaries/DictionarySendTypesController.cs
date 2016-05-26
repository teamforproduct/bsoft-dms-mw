using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Справочник типов рассылок (не модифицируется пользователем)
    /// </summary>
    [Authorize]
    public class DictionarySendTypesController : ApiController
    {
        /// <summary>
        /// Список типов рассылок
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        // GET: api/DictionarySendTypes
        public IHttpActionResult Get([FromUri] FilterDictionarySendType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionarySendTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Получение типа рассылки по ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/DictionarySendTypes/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionarySendType(ctx, id);
            return new JsonResult(tmpDict, this);
        }
    }
}