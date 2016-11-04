using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Logic.DocumentCore.Interfaces;
using System.Web.Http.Description;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentAccessesController : ApiController
    {
        /// <summary>
        /// Получение списка доступов по документу
        /// </summary>
        /// <param name="filter">модель фильтра</param>
        /// <param name="paging">paging</param>
        /// <returns></returns>
        [ResponseType(typeof(List<FrontDocumentAccess>))]
        public IHttpActionResult Get([FromUri] FilterDocumentAccess filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var docs = docProc.GetDocumentAccesses(ctx, filter, paging);
            var res = new JsonResult(docs, this);
            res.Paging = paging;
            return res;
        }
    }
}
