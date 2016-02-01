using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Documents
{
    public class DocumentRestrictedSendListsController : ApiController
    {
        /*
        // GET: api/DocumentRestrictedSendLists
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DocumentRestrictedSendLists/5
        public string Get(int id)
        {
            return "value";
        }
        */
        // POST: api/DocumentRestrictedSendLists
        public IHttpActionResult Post([FromBody]ModifyDocumentRestrictedSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var restrictedSendListId = docProc.AddRestrictedSendList(cxt, model);
            return new Results.JsonResult(restrictedSendListId, this);
        }
        /*
        // PUT: api/DocumentRestrictedSendLists/5
        public void Put(int id, [FromBody]string value)
        {
        }
        */
        // DELETE: api/DocumentRestrictedSendLists/5
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.DeleteRestrictedSendList(cxt, id);
            return new JsonResult(null, this);
        }
        
    }
}
