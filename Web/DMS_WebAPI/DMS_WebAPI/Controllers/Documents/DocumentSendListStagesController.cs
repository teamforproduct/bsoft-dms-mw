using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentSendListStagesController : ApiController
    {
        // POST: api/DocumentSendListStages
        public IHttpActionResult Post([FromBody]ModifyDocumentSendListStage model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            bool isLastStage =docProc.AddSendListStage(cxt, model);

            return Get(model.DocumentId, isLastStage);
        }

        // DELETE: api/DocumentSendListStages/5
        public IHttpActionResult Delete([FromBody]ModifyDocumentSendListStage model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.DeleteSendListStage(cxt, model);

            return Get(model.DocumentId);
        }

        private IHttpActionResult Get(int DocumentId, bool isLastStage = false)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetSendListStage(cxt, DocumentId, isLastStage), this);
        }
    }
}
