using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Actions;
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
    [RoutePrefix("api/DocumentActions")]
    public class DocumentActionsController : ApiController
    {
        // POST: api/DocumentActions/Favourite
        [Route("Favourite")]
        [HttpPost]
        public IHttpActionResult ChangeFavourites(ChangeFavourites model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ChangeFavouritesForDocument(cxt, model);
            return new JsonResult(null, this);
        }

        // POST: api/DocumentActions/ChangeWorkStatus
        [Route("ChangeWorkStatus")]
        [HttpPost]
        public IHttpActionResult ChangeWorkStatus(ChangeWorkStatus model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ChangeDocumentWorkStatus(cxt, model);
            return new JsonResult(null, this);
        }

        // POST: api/DocumentActions/AddNote
        [Route("AddNote")]
        [HttpPost]
        public IHttpActionResult AddNote(AddNote model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.AddDocumentComment(cxt, model);
            return new JsonResult(null, this);
        }

        // POST: api/DocumentActions/ControlOn
        [Route("ControlOn")]
        [HttpPost]
        public IHttpActionResult ControlOn(ControlOn model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ControlOn(cxt, model);
            return new JsonResult(null, this);
        }

        // POST: api/DocumentActions/CopyDocument
        [Route("CopyDocument")]
        [HttpPost]
        public IHttpActionResult Copy(CopyDocument model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(docProc.CopyDocument(cxt, model));
        }
    }
}
