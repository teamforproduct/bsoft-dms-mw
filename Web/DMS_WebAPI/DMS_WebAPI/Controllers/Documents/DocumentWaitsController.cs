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
    public class DocumentWaitsController : ApiController
    {
        /// <summary>
        /// Получение списка ожиданий 
        /// </summary>
        /// <param name="filter">модель фильтра ожиданий</param>
        /// <param name="paging">paging</param>
        /// <returns>список ожиданий</returns>
        [ResponseType(typeof(List<FrontDocumentWait>))]
        public IHttpActionResult Get([FromUri] FilterBase filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var waits = docProc.GetDocumentWaits(ctx, filter, paging);
            var res = new JsonResult(waits, this);
            res.Paging = paging;
            return res;
        }
    }
}