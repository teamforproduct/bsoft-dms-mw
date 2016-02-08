using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.IO;
using System.Web;
using System.Web.Http;
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
        public IHttpActionResult Post(int id, [FromUri] ModifyDocumentFile model)
        {
            model.DocumentId = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();

            int fileId = 0;

            var files = HttpContext.Current.Request.Files;
            for (int i = 0, l = files.Count; i < l; i++)
            {
                var file = files[i];

                byte[] fileData = null;
                //postedFile.InputStream.Read(fileData, 0, postedFile.ContentLength);
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    fileData = binaryReader.ReadBytes(file.ContentLength);
                }
                string fileName = file.FileName;

                fileId = docFileProc.AddUserFile(cxt, model.DocumentId, fileName, fileData, model.isAdditional);
            }

            //string testpath = @"C:\IFRToolLog.txt";
            //byte[] fileData = File.ReadAllBytes(testpath);
            //int fileId = docFileProc.AddUserFile(cxt, model.DocumentId, Path.GetFileName(testpath), fileData, model.isAdditional);

            return Get(fileId);
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

        // DELETE: api/DocumentFiles/5
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();

            docFileProc.DeleteDocumentFile(cxt, id, 0);
            return new JsonResult(null, this);
        }
    }
}
