using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryImportanceEventTypesController : ApiController
    {
        // GET: api/DictionaryImportanceEventTypes
        public IHttpActionResult Get([FromUri] FilterDictionaryImportanceEventType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryImportanceEventTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryImportanceEventTypes/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryImportanceEventType(ctx, id);
            return new JsonResult(tmpDict, this);
        }
    }
}