using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.SystemCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
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
                MessageLevelTypes = EnumMessageLevelTypes.Green,
                MessageLevelTypesName = EnumMessageLevelTypes.Green.ToString(),
                Message = "Успех, работаем на V3", //TODO 
            };
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }


    }
}