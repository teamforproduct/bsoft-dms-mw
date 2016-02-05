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
    public class DictionaryDocumentSubjectsController : ApiController
    {
        // GET: api/DictionaryDocumentSubjects
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentSubject filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDocumentSubjects(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryDocumentSubjects/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDocumentSubject(cxt, id);
            return new JsonResult(tmpDict, this);
        }
    }
}