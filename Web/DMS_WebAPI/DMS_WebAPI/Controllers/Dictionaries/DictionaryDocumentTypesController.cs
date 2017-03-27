using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using System.Collections.Generic;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryDocumentTypesController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Типы документов"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Типы документов"</param>
        /// <returns>FrontDictionaryDocumentType</returns>
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentType filter)
        {
            var tmpDicts = new List<FrontDictionaryDocumentType>();
            return new JsonResult(tmpDicts, this);
        }
    }
}