using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Database;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Инструменты. Доступ ограничен
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Tools)]
    public class SystemToolsController : ApiController
    {

        /// <summary>
        /// Реиндексация полнотекстового поиска для клиента
        /// </summary>
        /// <returns>сервер</returns>
        [HttpPost]
        [Route("FullTextReindex")]
        public IHttpActionResult FullTextReindex(int ClientId)
        {
            var dbProc = new WebAPIDbProcess();
            var clientServer = dbProc.GetClientServer(ClientId);
            DatabaseModel srv = dbProc.GetServer(clientServer.ServerId);
            srv.ClientId = clientServer.ClientId;
            var ctx = new AdminContext(srv);
            var ftService = DmsResolver.Current.Get<IFullTextSearchService>();
            ftService.ReindexDatabase(ctx);
            return new JsonResult(new FrontAdminServer { Id = ClientId }, this);
        }

    }
}