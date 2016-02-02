using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    [RoutePrefix("api/DictionaryLinkTypes")]
    public class DictionaryLinkTypesController : ApiController
    {
        // GET: api/DictionaryLinkTypes
        public IHttpActionResult Get([FromUri] FilterDictionaryLinkType filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryLinkTypes(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryLinkTypes/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryLinkType(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}