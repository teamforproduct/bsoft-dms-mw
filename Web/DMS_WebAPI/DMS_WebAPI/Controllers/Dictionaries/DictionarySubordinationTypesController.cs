using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionarySubordinationTypesController : ApiController
    {
        // GET: api/DictionarySubordinationTypes
        public IHttpActionResult Get([FromUri] FilterDictionarySubordinationType filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionarySubordinationTypes(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionarySubordinationTypes/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionarySubordinationType(ctx, id);
            return new JsonResult(tmpDict, this);
        }
    }
}