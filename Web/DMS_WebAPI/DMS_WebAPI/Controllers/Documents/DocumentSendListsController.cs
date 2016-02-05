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
    [Authorize]
    public class DocumentSendListsController : ApiController
    {
        // POST: api/DocumentSendLists
        public IHttpActionResult Post([FromBody]ModifyDocumentSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var restrictedSendListId = docProc.AddSendList(cxt, model);
            return new Results.JsonResult(restrictedSendListId, this);
        }
        
        // PUT: api/DocumentSendLists
        public IHttpActionResult Put([FromBody]ModifyDocumentSendListByStandartSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.AddSendListByStandartSendLists(cxt, model);
            return new Results.JsonResult(null, this);
        }
        // PUT: api/DocumentSendLists/5
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentSendList model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.UpdateSendList(cxt, model);
            return new Results.JsonResult(null, this);
        }

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
