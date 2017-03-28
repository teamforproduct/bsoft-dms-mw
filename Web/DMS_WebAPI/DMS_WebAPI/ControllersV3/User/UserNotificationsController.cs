using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using BL.Model.Users;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Настройки нотификации об изменениях в системе
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserNotificationsController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpItem = new FrontNotifications { EMailForNotifications = "t@t.t", IsSendEMail = true };
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает настройки нотификации
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Notifications)]
        [ResponseType(typeof(FrontNotifications))]
        public async Task<IHttpActionResult> Get()
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, -1);
            });
        }

        /// <summary>
        /// Корректирует настройки нотификации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Notifications)]
        public async Task<IHttpActionResult> Put([FromBody]ChangeNotifications model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, -1);
            });
        }

    }
}