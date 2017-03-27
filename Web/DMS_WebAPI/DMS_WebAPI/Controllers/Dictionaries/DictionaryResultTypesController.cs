using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using System.Web.Http;

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
            return new JsonResult(null, this);
        }
    }
}