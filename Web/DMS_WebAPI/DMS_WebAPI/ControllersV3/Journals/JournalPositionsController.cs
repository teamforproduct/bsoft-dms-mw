using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;

using BL.Model.Common;
using System.Web;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.DictionaryCore.FrontMainModel;
using System.Diagnostics;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.IncomingModel;

namespace DMS_WebAPI.ControllersV3.Journals
{
    /// <summary>
    /// Управление доступом должностей к журналам регистрации документов
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Journal)]
    public class JournalPositionsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список должностей, которые имеют доступ к журналу
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Positions)]
        [ResponseType(typeof(List<FrontDIPRegistrationJournalPositions>))]
        public IHttpActionResult Get([FromUri] int Id, [FromUri] FilterDictionaryPosition filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            //if (filter == null) filter = new FilterDictionaryPosition();
            //filter.IsChecked = true;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetPositionsByJournalDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }


    }
}
