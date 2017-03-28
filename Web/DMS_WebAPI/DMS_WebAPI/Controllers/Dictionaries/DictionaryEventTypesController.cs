using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using System.Collections.Generic;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryEventTypesController : ApiController
    {
        // GET: api/DictionaryEventTypes
        public IHttpActionResult Get([FromUri] FilterDictionaryEventType filter)
        {
            var tmpDicts = new List<FrontDictionaryEventType>();
            return new JsonResult(tmpDicts, this);
        }

    }
}