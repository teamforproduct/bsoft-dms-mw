using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Diagnostics;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentFavouriteController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Добавляет в Избранное
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Favourite + "/Add")]
        [HttpPut]
        public IHttpActionResult AddFavourite(ChangeFavourites model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.AddFavourite, model, model.CurrentPositionId);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Удаляет из Избранного
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Favourite+ "/Delete")]
        [HttpPut]
        public IHttpActionResult DeleteFavourite(ChangeFavourites model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteFavourite, model, model.CurrentPositionId);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}
