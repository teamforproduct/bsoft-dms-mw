using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DMS_WebAPI.Controllers
{
    public class TemplateDocumentsController : ApiController
    {
        // GET: api/TemplateDocuments
        public IHttpActionResult Get()
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docs = docProc.GetDocuments(
                new DefaultContext
                {
                    CurrentEmployee = new BL.Model.Users.Employee
                    {
                        Token = "1"
                    }
                }
                );
            return new JsonResult(docs, this);
            //return new DocumentsViewModel()
            //{
            //    Documents = new List<DocumentViewModel>() {
            //        new DocumentViewModel() {
            //            Id=10
            //        },
            //        new DocumentViewModel() {
            //            Id=15,
            //        }
            //    }
            //};
        }

        // GET: api/TemplateDocuments/5
        public IHttpActionResult Get(int id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var doc = docProc.GetDocument(
                new DefaultContext
                {
                    CurrentEmployee = new BL.Model.Users.Employee
                    {
                        Token = "1"
                    }
                }, id);
            return new JsonResult(doc, this);
        }

        // POST: api/TemplateDocuments
        public IHttpActionResult Post(BaseDocument model)
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.SaveDocument(new DefaultContext
            {
                CurrentEmployee = new BL.Model.Users.Employee
                {
                    Token = "1"
                }
            }, model);
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
