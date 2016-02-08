using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentAdditional;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentFilesController : ApiController
    {
        ////GET: api/DocumentFiles
        //public IHttpActionResult Get()
        //{
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
        //    return new JsonResult(docFileProc.Get(cxt, new DocumentAttachedFile()), this);
        //}

        // GET: api/DocumentFiles/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            return new JsonResult(docFileProc.GetDocumentFiles(cxt,id), this);
        }

        // POST: api/DocumentFiles
        public IHttpActionResult Post([FromBody]ModifyDocumentFile model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();

            return Get(docFileProc.AddUserFile(cxt, model));
        }

        // POST: api/DocumentFiles/GetFileData
        [Route("GetFileData")]
        [HttpPost]
        public IHttpActionResult GetFileData(DocumentAttachedFile model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = docFileProc.GetUserFile(cxt, model);
            return new JsonResult(res, this);
        }

        // PUT: api/DocumentFiles/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        // DELETE: api/DocumentFiles
        public IHttpActionResult Delete([FromBody]DocumentFileIdentity model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();

            docFileProc.DeleteDocumentFile(cxt, model);
            return new JsonResult(null, this);
        }
    }
}
