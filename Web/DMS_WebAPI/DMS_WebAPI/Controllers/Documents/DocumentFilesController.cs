using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Logic.DocumentCore.Interfaces;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.IO;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentFilesController : ApiController
    {
        //GET: api/DocumentFiles
        public IHttpActionResult Get([FromUri]FilterDocumentFileIdentity model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = docFileProc.GetUserFile(cxt, model);

            return new JsonResult(res, this);
        }

        // GET: api/DocumentFiles/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            return new JsonResult(docFileProc.GetDocumentFiles(cxt,id), this);
        }

        // POST: api/DocumentFiles
        public IHttpActionResult Post([FromBody]ModifyDocumentFiles model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();

            return new JsonResult(docFileProc.AddUserFile(cxt, model), this);
        }

        // PUT: api/DocumentFiles/5
        public IHttpActionResult Put([FromBody]ModifyDocumentFile model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();

            return new JsonResult(docFileProc.UpdateCurrentFileVersion(cxt, model), this);
        }

        // DELETE: api/DocumentFiles
        public IHttpActionResult Delete([FromUri]FilterDocumentFileIdentity model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();

            docFileProc.DeleteDocumentFile(cxt, model);
            return new JsonResult(null, this);
        }
    }
}
