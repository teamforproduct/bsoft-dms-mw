using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Models;
using DMS_WebAPI.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Documents")]
    public class DocumentsController : ApiController
    {
        //GET: api/Documents
        public IHttpActionResult Get()
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docs= docProc.GetDocuments();
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

        // GET: api/Documents/5
        public IHttpActionResult Get(int id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var doc = new { };
            //    docProc.GetDocument(new DefaultContext
            //{
            //    CurrentEmployee = new BL.Model.Users.Employee
            //    {
            //        Token = "1"
            //    }
            //}, id);
            return new JsonResult(doc, this);
        }

        // POST: api/Documents
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
