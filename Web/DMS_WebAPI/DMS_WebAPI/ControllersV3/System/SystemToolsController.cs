using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.Common;
using BL.Model.Database;
using BL.Model.SystemCore;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Инструменты. Доступ ограничен
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Tools)]
    public class SystemToolsController : ApiController
    {

        /// <summary>
        /// Реиндексация полнотекстового поиска для клиента
        /// </summary>
        /// <returns>сервер</returns>
        [HttpPost]
        [Route(Features.Info + "/FullTextReindex")]
        public IHttpActionResult FullTextReindex([FromBody]Item model)
        {
            var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
            var clientServer = dbProc.GetClientServer(model.Id);
            DatabaseModel srv = dbProc.GetServer(clientServer.ServerId);
            srv.ClientId = clientServer.ClientId;
            var ctx = new AdminContext(srv);
            var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
            ftService.ReindexDatabase(ctx);
            return new JsonResult(new FrontAdminServer { Id = model.Id }, this);
        }

    }
}