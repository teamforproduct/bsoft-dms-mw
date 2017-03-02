using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Diagnostics;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Должности.
    /// Доступ к документам
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Position)]
    public class PositionDocumentAccessesController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Заменяет должность по документу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.DocumentAccesses)]
        public IHttpActionResult ChangePosition([FromBody]ChangePosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.ChangePosition, model);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
