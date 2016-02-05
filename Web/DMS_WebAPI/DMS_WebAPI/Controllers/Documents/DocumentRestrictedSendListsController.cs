using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentRestrictedSendListsController : ApiController
    {
        // POST: api/DocumentRestrictedSendLists
        public IHttpActionResult Post([FromBody]ModifyDocumentRestrictedSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var restrictedSendListId = docProc.AddRestrictedSendList(cxt, model);
            return new Results.JsonResult(restrictedSendListId, this);
        }
        
        // PUT: api/DocumentRestrictedSendLists
        public IHttpActionResult Put([FromBody]ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.AddRestrictedSendListByStandartSendLists(cxt, model);
            return new Results.JsonResult(null, this);
        }
        
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
