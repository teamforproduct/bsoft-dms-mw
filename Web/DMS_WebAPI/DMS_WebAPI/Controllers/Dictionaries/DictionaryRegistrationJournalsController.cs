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
    [RoutePrefix("api/DictionaryRegistrationJournals")]
    public class DictionaryRegistrationJournalsController : ApiController
    {
        // GET: api/DictionaryRegistrationJournals
        public IHttpActionResult Get([FromUri] FilterDictionaryRegistrationJournal filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryRegistrationJournals(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryRegistrationJournals/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryRegistrationJournal(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}