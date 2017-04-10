using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.Database;
using BL.Model.SystemCore;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Threading.Tasks;
using System.Web.Http;
using BL.Database.DatabaseContext;
using Ninject;
using Ninject.Parameters;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Инструменты. Доступ ограничен
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Tools)]
    public class SystemToolsController : WebApiController
    {

        /// <summary>
        /// Реиндексация полнотекстового поиска для клиента
        /// </summary>
        /// <returns>сервер</returns>
        [HttpPost]
        [Route(Features.Info + "/FullTextReindex")]
        public async Task<IHttpActionResult> FullTextReindex()
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var dbProc = DmsResolver.Current.Get<WebAPIDbProcess>();
                var clientServer = dbProc.GetClientServer(context.CurrentClientId);
                DatabaseModel srv = dbProc.GetServer(clientServer.ServerId);
                srv.ClientId = clientServer.ClientId;

                var ctx = new AdminContext(srv);
                var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
                ftService.ReindexDatabase(ctx);
                return new JsonResult(new FrontAdminServer { Id = ctx.CurrentClientId }, this);
            });
        }

    }
}