using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Threading.Tasks;
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
        /// <summary>
        /// Заменяет должность по документу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.DocumentAccesses)]
        public async Task<IHttpActionResult> ChangePosition([FromBody]ChangePosition model)
        {
            Action.Execute(EnumDocumentActions.ChangePosition, model);
            var res = new JsonResult(null, this);
            return res;
        }

    }
}
