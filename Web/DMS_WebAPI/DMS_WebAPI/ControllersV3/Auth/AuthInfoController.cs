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

            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.ChangeLoginAgentUserAsync(context, model);

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

            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.ConfirmEmailAgentUser(context, model.Id);

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

            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.ChangePasswordAgentUserAsync(context, model);

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
        public IHttpActionResult ChangeLockout([FromBody]ChangeLockoutAgentUser model)
        {
            if (!ModelState.IsValid) return new JsonResult(ModelState, false, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();

            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            webService.ChangeLokoutAgentUserAsync(context, model);

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
            var contexts = DmsResolver.Current.Get<UserContexts>();
            var context = contexts.Get();

            contexts.RemoveByClientId(context.Client.Id, model.Id);

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

            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            await webService.SetMustChangePasswordAgentUserAsync(context, model);

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
