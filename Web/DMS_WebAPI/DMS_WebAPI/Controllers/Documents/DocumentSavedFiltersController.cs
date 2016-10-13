using BL.Logic.DocumentCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.DocumentCore.Filters;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentSavedFiltersController : ApiController
    {
        // GET: api/DocumentSavedFilters
        public IHttpActionResult Get([FromUri] FilterDocumentSavedFilter filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var savFilters = docProc.GetSavedFilters(ctx, filter);
            return new JsonResult(savFilters, this);
        }

        // GET: api/DocumentSavedFilters/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var savFilter = docProc.GetSavedFilter(ctx, id);
            return new JsonResult(savFilter, this);
        }

        // POST: api/DocumentSavedFilters
        public IHttpActionResult Post([FromBody]ModifyDocumentSavedFilter model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var id = (int)docProc.ExecuteAction(EnumDocumentActions.AddSavedFilter, ctx, model);
            return Get(id);
        }

        // PUT: api/DocumentSavedFilters/5
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentSavedFilter model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifySavedFilter, ctx, model);
            return Get(model.Id);
        }

        // DELETE: api/DocumentSavedFilters/5
        public IHttpActionResult Delete(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteSavedFilter, ctx, id);
            return new JsonResult(null, this);
        }
    }
}
