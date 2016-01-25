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
        // GET: api/Documents
        public DocumentsViewModel Get()
        {
            return new DocumentsViewModel() {
                Documents = new List<DocumentViewModel>() {
                    new DocumentViewModel() {
                        Id=10
                    },
                    new DocumentViewModel() {
                        Id=15,
                    }
                }
            };
        }

        // GET: api/Documents/5
        public IHttpActionResult Get(int id)
        {
            return new JsonResult(new { test=id }, this);
        }

        // POST: api/Documents
        public void Post([FromBody]string value)
        {
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
