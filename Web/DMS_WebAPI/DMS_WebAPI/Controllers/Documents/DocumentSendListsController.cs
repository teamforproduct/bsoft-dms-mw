using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Documents
{
    public class DocumentSendListsController : ApiController
    {
        /*
        // GET: api/DocumentSendLists
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DocumentSendLists/5
        public string Get(int id)
        {
            return "value";
        }
        */
        // POST: api/DocumentSendLists
        public IHttpActionResult Post([FromBody]ModifyDocumentSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var restrictedSendListId = docProc.AddSendList(cxt, model);
            return new Results.JsonResult(restrictedSendListId, this);
        }
        /*
        // PUT: api/DocumentSendLists/5
        public void Put(int id, [FromBody]string value)
        {
        }
        */
        // DELETE: api/DocumentSendLists/5
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.DeleteSendList(cxt, id);
            return new JsonResult(null, this);
        }
    }
}
