using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;

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