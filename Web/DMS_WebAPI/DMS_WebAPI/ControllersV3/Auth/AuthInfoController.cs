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
using BL.Model.FullTextSearch;
using System.Threading.Tasks;
using DMS_WebAPI.Models;
using BL.Model.Exception;
using BL.Model.Users;
using BL.Logic.AdminCore.Interfaces;
using Microsoft.AspNet.Identity.Owin;

namespace DMS_WebAPI.ControllersV3.Auth
{
    /// <summary>
    /// Управление авторизацией сотрудников-пользователей
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Auth)]
    public class AuthInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает авторизационные сведения сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/{Id:int}")]
        [ResponseType(typeof(FrontAuthInfo))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();

            var webService = new WebAPIService();

            ApplicationUser user = await webService.GetUserAsync(ctx, Id);

            if (user == null) throw new UserIsNotDefined();

            var authInfo = new FrontAuthInfo
            {
                Id = Id,
                Email = user.Email,
                Login = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                IsLockout = user.IsLockout,
               
            };

            var res = new JsonResult(authInfo, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Измененяет логин
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info/ChangeLogin")]
        public async Task<IHttpActionResult> ChangeLogin([FromBody]ChangeLoginAgentUser model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = new WebAPIService();
            webService.ChangeLoginAgentUser(model);

            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Измененяет пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info/ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword([FromBody]ChangePasswordAgentUser model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = new WebAPIService();

            await webService.ChangePasswordAgentUserAsync(model);

            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Управляет блокировкой пользователя (управляет возможностью войти в систему)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info/ChangeLockout")]
        public async Task<IHttpActionResult> ChangeLockoutAgentUser([FromBody]ChangeLockoutAgentUser model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = new WebAPIService();
            await webService.ChangeLockoutAgentUserAsync(ctx, model);

            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Убиение всех активных сессий пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info/KillSessionsAgentUser")]
        public IHttpActionResult KillSessionsAgentUser([FromBody]Item model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var userContexts = DmsResolver.Current.Get<UserContexts>();
            var ctx = userContexts.Get();

            var admService = DmsResolver.Current.Get<IAdminService>();
            admService.ExecuteAction(EnumAdminActions.KillSessions, ctx, model.Id);

            userContexts.RemoveByAgentId(model.Id);

            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Заставляет пользователя сменить пароль при первом входе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info/SetMustChangePassword")]
        public async Task<IHttpActionResult> SetMustChangePasswordAgentUser([FromBody]MustChangePasswordAgentUser model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            var mngContext = DmsResolver.Current.Get<UserContexts>();

            var ctx = mngContext.Get();

            var admService = DmsResolver.Current.Get<IAdminService>();
            var userId = (string)admService.ExecuteAction(EnumAdminActions.MustChangePassword, ctx, model.Id);

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new UserIsNotDefined();

            user.IsChangePasswordRequired = model.MustChangePassword;//true;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new JsonResult(result, false, string.Join(" ", result.Errors), this);
                //return GetErrorResult(result);
            }
            return new JsonResult(null, this);
        }


    }
}
