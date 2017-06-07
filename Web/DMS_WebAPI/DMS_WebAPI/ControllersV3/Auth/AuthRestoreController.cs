using BL.CrossCutting.DependencyInjection;
using BL.Model.SystemCore;
using BL.Model.Users;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.Auth
{
    /// <summary>
    /// Авторизация. Восстановление авторизациии сотрудников-пользователей
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Auth)]
    public class AuthRestoreController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        

        /// <summary>
        /// Подтверждает адрес пользователя
        /// Это апи отрабатывает по ссылке из письма
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IHttpActionResult> SetConfirmEmail([FromBody] ConfirmEmail model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.ConfirmEmail(model.UserId, model.Code);

            var user = await webService.GetUserByIdAsync(model.UserId);
            var res = new JsonResult(new BL.Model.Context.User { Id = user.Id, Name = user.UserName, IsChangePasswordRequired = user.IsChangePasswordRequired }, this);

            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("RestorePassword")]
        public async Task<IHttpActionResult> RestorePassword(RestorePassword model)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.RestorePassword(model);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Сбрасывает пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword([FromBody]ResetPassword model)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var res = await webService.ResetPassword(model);
            return new JsonResult(new { UserName = res }, this);
        }

        /// <summary>
        /// Сбрасывает пароль
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("ValidatePassword")]
        public async Task<IHttpActionResult> ValidatePassword([FromBody]string password)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.ValidatePassword(password);
            return new JsonResult(null, this);
        }

    }
}
