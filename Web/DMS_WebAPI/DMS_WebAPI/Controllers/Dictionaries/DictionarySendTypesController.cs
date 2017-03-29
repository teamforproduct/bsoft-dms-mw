using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using System.Web.Http;

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
            return new JsonResult(null, this);
        }

    }
}