using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.Users;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using Microsoft.AspNet.Identity.Owin;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Auth
{
    /// <summary>
    /// Авторизация. Управление авторизацией сотрудников-пользователей
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Auth)]
    public class AuthInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает авторизационные сведения сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontAuthInfo))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();

            var webService = DmsResolver.Current.Get<WebAPIService>();

            AspNetUsers user = await webService.GetUserAsync(ctx, Id);

            if (user == null) throw new UserIsNotDefined();

            var authInfo = new FrontAuthInfo
            {
                Id = Id,
                Email = user.Email,
                Login = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEndDate = user.LockoutEndDateUtc
            };

            var res = new JsonResult(authInfo, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Администратор меняет логин - жуть
        /// Выбрасывает пользователя из всех его клиентов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info + "/ChangeLogin")]
        public async Task<IHttpActionResult> ChangeLogin([FromBody]ChangeLoginAgentUser model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.ChangeLoginAgentUser(model);

            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Высылает письмо для подтверждения email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info + "/ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail([FromBody]Item model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.ConfirmEmailAgentUser(model.Id);

            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }



        /// <summary>
        /// Администратор мененяет пароль пользователю, жуть - чтобы под ним зайти наверное
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info + "/ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword([FromBody]ChangePasswordAgentUser model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = DmsResolver.Current.Get<WebAPIService>();

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
        [Route(Features.Info + "/ChangeLockout")]
        public async Task<IHttpActionResult> ChangeLockout([FromBody]ChangeLockoutAgentUser model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();

            if (model.IsLockout)
            {
                var userContexts = DmsResolver.Current.Get<UserContexts>();

                var user = webService.GetUser(ctx, model.Id);

                if (user == null) throw new UserIsNotDefined();

                userContexts.RemoveByClientId(ctx.Client.Id, user.Id);
            }

            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Убиение всех активных сессий пользователя в текущем клиенте
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info + "/KillSessions")]
        public IHttpActionResult KillSessions([FromBody]Item model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = DmsResolver.Current.Get<WebAPIService>();
            var userContexts = DmsResolver.Current.Get<UserContexts>();
            var ctx = userContexts.Get();

            var user = webService.GetUser(ctx, model.Id);

            if (user == null) throw new UserIsNotDefined();

            userContexts.RemoveByClientId(ctx.Client.Id, user.Id);

            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Заставляет пользователя сменить пароль при первом входе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // TODO перейти на UserId c AgentId 
        [HttpPut]
        [Route(Features.Info + "/SetMustChangePassword")]
        public async Task<IHttpActionResult> SetMustChangePassword([FromBody]MustChangePasswordAgentUser model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            var mngContext = DmsResolver.Current.Get<UserContexts>();

            var ctx = mngContext.Get();

            var admService = DmsResolver.Current.Get<IAdminService>();
            var userId = (string)admService.ExecuteAction(EnumAdminActions.MustChangePassword, ctx, model.Id);

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            AspNetUsers user = await userManager.FindByIdAsync(userId);

            if (user == null) throw new UserIsNotDefined();

            user.IsChangePasswordRequired = model.MustChangePassword;//true;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new JsonResult(result, false, string.Join(" ", result.Errors), this);
                //return GetErrorResult(result);
            }
            return new JsonResult(null, this);
        }


        /// <summary>
        /// Выключает безопасный вход (fingerprint) указанному пользователю
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info + "/SwitchOffFingerprint")]
        // TODO перейти на UserId c AgentId 
        public async Task<IHttpActionResult> SwitchOffFingerprint([FromBody]Item model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.SwitchOffFingerprint(ctx, model);

            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
