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
        /// <summary>
        /// use v3
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri] FilterDocumentSavedFilter filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var savFilters = docProc.GetSavedFilters(ctx, filter);
            return new JsonResult(savFilters, this);
        }

        /// <summary>
        /// use v3
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFiltersService>();
            var savFilter = docProc.GetSavedFilter(ctx, id);
            return new JsonResult(savFilter, this);
        }

        /// <summary>
        /// use v3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]AddDocumentSavedFilter model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var id = (int)docProc.ExecuteAction(EnumDocumentActions.AddSavedFilter, ctx, new ModifyDocumentSavedFilter(model));
            return Get(id);
        }

        /// <summary>
        /// use v3
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentSavedFilter model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifySavedFilter, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// use v3
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteSavedFilter, ctx, id);
            return new JsonResult(null, this);
        }
    }
}
