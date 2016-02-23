using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionarySendTypesController : ApiController
    {
        // GET: api/DictionarySendTypes
        public IHttpActionResult Get([FromUri] FilterDictionarySendType filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionarySendTypes(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionarySendTypes/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionarySendType(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}