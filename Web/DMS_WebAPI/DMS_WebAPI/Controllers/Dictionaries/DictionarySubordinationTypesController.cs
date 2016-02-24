using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionarySubordinationTypesController : ApiController
    {
        // GET: api/DictionarySubordinationTypes
        public IHttpActionResult Get([FromUri] FilterDictionarySubordinationType filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionarySubordinationTypes(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionarySubordinationTypes/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionarySubordinationType(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}