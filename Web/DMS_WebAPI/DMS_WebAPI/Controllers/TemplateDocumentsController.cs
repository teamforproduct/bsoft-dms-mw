using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/TemplateDocuments")]
    public class TemplateDocumentsController : ApiController
    {
        // GET: api/TemplateDocuments
        public IHttpActionResult Get()
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDocs = tmpDocProc.GetDocumentTemplates(cxt);
            return new JsonResult(tmpDocs, this);
        }

        // GET: api/TemplateDocuments/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDoc = tmpDocProc.GetDocumentTemplates(cxt).FirstOrDefault();
            return new JsonResult(tmpDoc, this);
        }

        // POST: api/TemplateDocuments
        public IHttpActionResult Post(TemplateDocumentGet model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            docProc.AddOrUpdateTemplate(cxt, model);
            return Ok();
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
