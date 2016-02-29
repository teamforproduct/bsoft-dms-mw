using BL.Logic.DependencyInjection;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentFilesController : ApiController
    {
        //GET: api/Files
        public IHttpActionResult Get([FromUri]FilterDocumentFileIdentity model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = docFileProc.GetUserFile(cxt, model);

            return new JsonResult(res, this);
        }

        // GET: api/Files/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            return new JsonResult(docFileProc.GetDocumentFiles(cxt,id), this);
        }

        // POST: api/Files
        public IHttpActionResult Post([FromBody]ModifyDocumentFiles model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, cxt, model);
            return Get(model.DocumentId);
        }

        // PUT: api/Files/5
        public IHttpActionResult Put([FromBody]ModifyDocumentFile model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var fl = (FrontDocumentAttachedFile)docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentFile, cxt, model);
           
            return new JsonResult(fl, this);
        }

        // DELETE: api/Files
        public IHttpActionResult Delete([FromUri]FilterDocumentFileIdentity model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentFile, cxt, model);
            return new JsonResult(null, this);
        }
    }
}
