using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;
using BL.Model.Common;
using System.Diagnostics;
using BL.Model.SystemCore;
using BL.Model.SystemCore.FrontModel;

namespace DMS_WebAPI.ControllersV3.Licences
{
    /// <summary>
    /// Проверка лицензии
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Licences)]
    public class LicencesCheckController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();


        /// <summary>
        /// Проверка лицензии
        /// </summary>
        /// <returns>список должностей</returns>
        [HttpGet]
        [Route("Check")]
        [ResponseType(typeof(FrontSystemLicencesInfo))]
        public IHttpActionResult VerifyLicences()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var context = DmsResolver.Current.Get<UserContexts>().Get(keepAlive: false);
            var tmpItem = new FrontSystemLicencesInfo
            {
                MessageLevelTypes = EnumMessageLevelTypes.Red,
                MessageLevelTypesName = EnumMessageLevelTypes.Red.ToString(),
                Message = "Ваша лицензия на V2 заканчивается. Пожалуйста, перейдите на V3", //TODO 
            };
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }


    }
}