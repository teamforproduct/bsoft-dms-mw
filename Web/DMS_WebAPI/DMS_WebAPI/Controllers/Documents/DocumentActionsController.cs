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
    }
}
