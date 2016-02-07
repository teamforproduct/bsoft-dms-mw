using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.DocumentAdditional;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentFilesController : ApiController
    {
        // GET: api/DocumentFiles
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/DocumentFiles/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            return new JsonResult(docFileProc.GetUserFile(cxt,new DocumentAttachedFile()), this);
        }

        // POST: api/DocumentFiles
        public IHttpActionResult Post(int id, [FromUri]ModifyDocumentFile model)
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

            return Get(fileId);
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
