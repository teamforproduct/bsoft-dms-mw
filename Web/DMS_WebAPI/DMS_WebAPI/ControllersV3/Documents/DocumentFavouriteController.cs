using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Reports.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

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
        [Route(Features.Favourite)]
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
        [Route(Features.Favourite)]
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
