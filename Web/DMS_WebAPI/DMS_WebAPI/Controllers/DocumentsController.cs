using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web;
using System.Web.Http;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Documents")]
    public class DocumentsController : ApiController
    {
        //GET: api/Documents
        public IHttpActionResult Get([FromUri] FilterDocument filters)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docs = docProc.GetDocuments(cxt, filters);
            return new JsonResult(docs, this);
        }

        // GET: api/Documents/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var doc = docProc.GetDocument(cxt, id);
            return new JsonResult(doc, this);
        }

        // POST: api/Documents
        public IHttpActionResult Post(int templateId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.AddDocumentByTemplateDocument(cxt, templateId);
            return Ok();
        }

        // PUT: api/Documents/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Documents/5
        public void Delete(int id)
        {
        }
    }
}
