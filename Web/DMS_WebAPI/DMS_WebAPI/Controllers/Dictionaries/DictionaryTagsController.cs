using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.SystemCore;
using BL.Model.FullTextSearch;

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