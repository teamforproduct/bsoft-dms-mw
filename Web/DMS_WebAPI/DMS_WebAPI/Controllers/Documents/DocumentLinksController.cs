using System.Web.Http;
using System.Web.Http.Description;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentLinksController : ApiController
    {
        /// <summary>
        /// Список id связоных документов
        /// </summary>
        /// <param name="id">ID Документа</param>
        /// <returns>Cписок id связоных документов</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetLinkedDocumentIds(ctx, id), this);
        }
    }
}