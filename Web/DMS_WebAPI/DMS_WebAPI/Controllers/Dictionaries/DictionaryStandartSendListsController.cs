using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore;
using BL.Model.DictionaryCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    [RoutePrefix("api/DictionaryStandartSendLists")]
    public class DictionaryStandartSendListsController : ApiController
    {
        // GET: api/TemplateDocuments
        public IHttpActionResult Get([FromUri] FilterDictionaryStandartSendList filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryStandartSendList(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/TemplateDocuments/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryStandartSendList(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        // POST: api/TemplateDocuments
        public void Post(BaseTemplateDocument model)
        {
        }

        // PUT: api/TemplateDocuments/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/TemplateDocuments/5
        public void Delete(int id)
        {
        }
    }
}