using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers.Dictionaries
{

    /// <summary>
    /// Системный справочник, описывает типы документов по направлению:
    /// 1 - Входящие;
    /// 2 -	Исходящие;
    /// 3 -	Внутренние;
    /// </summary>
    [Authorize]
    public class DictionaryDocumentDirectionsController : ApiController
    {
        /// <summary>
        /// Направления документов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<FrontDictionaryDocumentDirection>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentDirection filter)
        {
            var tmpDicts = new List<FrontDictionaryDocumentDirection>();
            return new JsonResult(tmpDicts, this);
        }
    }
}