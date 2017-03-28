using BL.Model.DictionaryCore.FilterModel;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryTagsController : ApiController
    {
        // GET: api/DictionaryTags
        /// <summary>
        /// Получить список доступных тегов для выставленых должностей
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns>Список доступных тегов для выставленых должностей</returns>
        public IHttpActionResult Get([FromUri]FullTextSearch ftSearch, [FromUri]FilterDictionaryTag filter, UIPaging paging)
        {
            return new JsonResult(null, this);
        }

    }
}