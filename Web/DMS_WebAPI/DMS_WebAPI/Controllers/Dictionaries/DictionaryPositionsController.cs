using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers.Dictionaries
{

    /// <summary>
    /// Описывает должности в отделах.
    /// Должности всегда подчинены отделам.
    /// Значимость должносьти в отделе задается параметром Order
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DictionaryPositions")]
    public class DictionaryPositionsController : ApiController
    {

        /// <summary>
        /// Возвращает записи из словаря "Должности"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Должности"</param>
        /// <returns>FrontDictionaryPositions</returns>
        [ResponseType(typeof(List<FrontDictionaryPosition>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryPosition filter)
        {
            var res = new JsonResult(null, this);
            return res;
        }

    }
}