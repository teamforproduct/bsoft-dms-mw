using DMS_WebAPI.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Documents")]
    public class DocumentsController : ApiController
    {
        // GET: api/Documents
        public IHttpActionResult Get()
        {
            return new JsonResult(new object[] { },this);
        }

        // GET: api/Documents/5
        public IHttpActionResult Get(int id)
        {
            return new JsonResult(new { }, this);
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
