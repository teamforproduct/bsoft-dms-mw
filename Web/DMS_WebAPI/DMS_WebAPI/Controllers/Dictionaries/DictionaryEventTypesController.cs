using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using BL.Model.DictionaryCore.FrontModel;

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