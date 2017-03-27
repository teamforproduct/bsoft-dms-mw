using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.SystemCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Лицензия. Проверка лицензии
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Licences)]
    public class LicencesCheckController : ApiController
    {
        /// <summary>
        /// Проверка лицензии
        /// </summary>
        /// <returns>список должностей</returns>
        [HttpGet]
        [Route("Check")]
        [ResponseType(typeof(FrontSystemLicencesInfo))]
        public async Task<IHttpActionResult> VerifyLicences()
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get(keepAlive: false);
            var tmpItem = new FrontSystemLicencesInfo
            {
                MessageLevelTypes = EnumMessageLevelTypes.Green,
                MessageLevelTypesName = EnumMessageLevelTypes.Green.ToString(),
                Message = "Успех, работаем на V3", //TODO 
            };
            var res = new JsonResult(tmpItem, this);
            return res;
        }


    }
}