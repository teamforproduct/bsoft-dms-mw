using BL.Logic.DependencyInjection;
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
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentSavedFiltersController : ApiController
    {
        // GET: api/DocumentSavedFilters
        public IHttpActionResult Get()
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var savFilters = docProc.GetSavedFilters(cxt);
            return new JsonResult(savFilters, this);
        }

        // GET: api/DocumentSavedFilters/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var savFilter = docProc.GetSavedFilter(cxt, id);
            return new JsonResult(savFilter, this);
        }

        // POST: api/DocumentSavedFilters
        public IHttpActionResult Post([FromBody]ModifyDocumentSavedFilter model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var id = (int)docProc.ExecuteAction(EnumDocumentActions.AddSavedFilter, cxt, model);
            return Get(id);
        }

        // PUT: api/DocumentSavedFilters/5
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentSavedFilter model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifySavedFilter, cxt, model);
            return Get(model.Id);
        }

        // DELETE: api/DocumentSavedFilters/5
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteSavedFilter, cxt, id);
            return new JsonResult(null, this);
        }
    }
}
