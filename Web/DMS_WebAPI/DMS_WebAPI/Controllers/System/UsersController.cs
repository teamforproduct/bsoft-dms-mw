using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Exception;
using BL.Logic.DictionaryCore.Interfaces;
using System.Linq;
using Microsoft.AspNet.Identity;
using BL.Model.WebAPI.IncomingModel;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
using System.Web;
using System;
using System.Web.Http.Description;
using BL.Model.AdminCore.FrontModel;
using BL.Model.DictionaryCore.FrontModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using BL.Model.Users;
using DMS_WebAPI.Models;
using BL.Logic.SystemServices.MailWorker;
using System.Configuration;
using BL.CrossCutting.Context;
using BL.Model.Constants;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/v2/Users")]
    public class UsersController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Получение информации о пользователе
        /// </summary>
        /// <returns>список должностей</returns>
        [Route("UserInfo")]
        [HttpGet]
        [ResponseType(typeof(FrontDictionaryAgent))]
        public IHttpActionResult GetUserInfo()
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var dicProc = DmsResolver.Current.Get<IDictionaryService>();

            var agent = dicProc.GetDictionaryAgent(context, context.CurrentAgentId);

            return new JsonResult(agent, this);
        }

        /// <summary>
        /// Получение списка должностей, доступных текущего для пользователя
        /// </summary>
        /// <returns>список должностей</returns>
        [Route("AvailablePositions")]
        [HttpGet]
        [ResponseType(typeof(List<FrontAvailablePositions>))]
        public IHttpActionResult AvailablePositions()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAvailablePositions(context);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Получение массива ИД должностей, выбранных текущим пользователем
        /// </summary>
        /// <returns>массива ИД должностей</returns>
        [Route("ChoosenPositions")]
        [HttpGet]
        [ResponseType(typeof(List<int>))]
        public IHttpActionResult ChoosenPositions()
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            return new JsonResult(context.CurrentPositionsIdList, this);
        }

        /// <summary>
        /// Установка должностей, от которых будет работать текущий пользователь
        /// </summary>
        /// <param name="positionsIdList">ИД должности</param>
        /// <returns></returns>
        [Route("ChoosenPositions")]
        public IHttpActionResult Post([FromBody]List<int> positionsIdList)
        {
            var user_context = DmsResolver.Current.Get<UserContexts>();
            var context = user_context.Get();
            var admProc = DmsResolver.Current.Get<IAdminService>();
            admProc.VerifyAccess(context, new VerifyAccess() { PositionsIdList = positionsIdList });
            user_context.SetUserPositions(context.CurrentEmployee.Token, positionsIdList);
            //context.CurrentPositionsIdList = positionsIdList;
            //ctx.CurrentPositions = new List<CurrentPosition>() { new CurrentPosition { CurrentPositionId = positionId } };
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Получение списка доступных серверов
        /// </summary>
        /// <returns>список серверов</returns>
        [Route("Servers")]
        [HttpGet]
        public IHttpActionResult GetServers()
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();

            var dbProc = new WebAPIDbProcess();

            var servers = dbProc.GetServersByUser(context);

            return new JsonResult(servers, this);
        }

        /// <summary>
        /// Установить сервер для использования
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Servers")]
        [HttpPost]
        public async Task<IHttpActionResult> SetServers([FromBody]SetUserServer model)
        {
            var mngContext = DmsResolver.Current.Get<UserContexts>();



            var dbProc = new WebAPIDbProcess();

            // Получаю первый попавшийся сервер, в который сконфигурен пользователь
            var db = dbProc.GetServerByUser(User.Identity.GetUserId(), model);
            if (db == null)
            {
                throw new DatabaseIsNotFound();
            }

            mngContext.Set(db, model.ClientId);
            var ctx = mngContext.Get();

            var logger = DmsResolver.Current.Get<ILogger>();
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            var userAgent = HttpContext.Current.Request.UserAgent;
            var mobile = userAgent.Contains("Mobile") ? "Mobile; " : string.Empty;
            var message = $"{HttpContext.Current.Request.UserHostAddress}; {bc.Browser} {bc.Version}; {bc.Platform}; {mobile}";
            logger.Information(ctx, message, (int)EnumObjects.System, (int)EnumSystemActions.Login);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Получение списка доступных клиентов для пользователя
        /// </summary>
        /// <returns>список серверов</returns>
        [Route("Clients")]
        [HttpGet]
        public IHttpActionResult GetClients()
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();

            var dbProc = new WebAPIDbProcess();

            var clients = dbProc.GetClientsByUser(context);

            return new JsonResult(clients, this);
        }

        /// <summary>
        /// Установить клиента для пользователя
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [Route("Clients")]
        [HttpPost]
        public IHttpActionResult SetClients([FromBody]int clientId)
        {
            var mngContext = DmsResolver.Current.Get<UserContexts>();

            var ctx = mngContext.Get();

            var dbProc = new WebAPIDbProcess();

            var client = dbProc.GetClientByUser(User.Identity.GetUserId(), clientId);
            if (client == null)
            {
                throw new ClientIsNotFound();
            }

            mngContext.Set(client);


            return new JsonResult(null, this);
        }

        /// <summary>
        /// Изменение пароля
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ChangePasswordAgentUser")]
        [HttpPost]
        public async Task<IHttpActionResult> ChangePasswordAgentUser(ChangePasswordAgentUser model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mngContext = DmsResolver.Current.Get<UserContexts>();

            var ctx = mngContext.Get();

            var admService = DmsResolver.Current.Get<IAdminService>();
            var userId = (string)admService.ExecuteAction(EnumAdminActions.ChangePasswordAgentUser, ctx, model.AgentId);

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var token = await userManager.GeneratePasswordResetTokenAsync(userId);

            var result = await userManager.ResetPasswordAsync(userId, token, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            if (model.IsChangePasswordRequired)
            {
                ApplicationUser user = await userManager.FindByIdAsync(userId);

                if (user == null)
                    throw new UserNameIsNotDefined();

                user.IsChangePasswordRequired = true;

                result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }

            if (model.IsKillSessions)
                mngContext.KillSessions(model.AgentId);

            return new JsonResult(null, this);
        }

        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ChangeLockoutAgentUser")]
        [HttpPut]
        public async Task<IHttpActionResult> ChangeLockoutAgentUser(ChangeLockoutAgentUser model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mngContext = DmsResolver.Current.Get<UserContexts>();

            var ctx = mngContext.Get();

            var admService = DmsResolver.Current.Get<IAdminService>();
            var userId = (string)admService.ExecuteAction(EnumAdminActions.ChangeLockoutAgentUser, ctx, model.AgentId);

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new UserNameIsNotDefined();

            user.IsLockout = model.IsLockout;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            if (model.IsKillSessions)
                mngContext.KillSessions(model.AgentId);

            return new JsonResult(null, this);
        }

        /// <summary>
        /// Убиение всех активных сессий пользователя
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [Route("KillSessionsAgentUser")]
        [HttpPut]
        public IHttpActionResult KillSessionsAgentUser(int agentId)
        {
            var mngContext = DmsResolver.Current.Get<UserContexts>();

            var ctx = mngContext.Get();

            var admService = DmsResolver.Current.Get<IAdminService>();
            var userId = (string)admService.ExecuteAction(EnumAdminActions.KillSessionsAgentUser, ctx, agentId);

            mngContext.KillSessions(agentId);

            return new JsonResult(null, this);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ChangeLoginAgentUser")]
        [HttpPost]
        public async Task<IHttpActionResult> ChangeLoginAgentUser(ChangeLoginAgentUser model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mngContext = DmsResolver.Current.Get<UserContexts>();

            var ctx = mngContext.Get();

            var admService = DmsResolver.Current.Get<IAdminService>();
            var userId = (string)admService.ExecuteAction(EnumAdminActions.ChangeLoginAgentUser, ctx, model.AgentId);

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new UserNameIsNotDefined();

            user.UserName = model.NewEmail;
            user.Email = model.NewEmail;
            user.IsEmailConfirmRequired = model.IsVerificationRequired;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            mngContext.KillSessions(model.AgentId);

            if (model.IsVerificationRequired)
            {
                var emailConfirmationCode = await userManager.GenerateEmailConfirmationTokenAsync(user.Id);

                var callbackurl = new Uri(new Uri(ConfigurationManager.AppSettings["WebSiteUrl"]), "/api/v2/Users/ConfirmEmailAgentUser").AbsoluteUri;

                callbackurl += String.Format("?userId={0}&code={1}", user.Id, HttpUtility.UrlEncode(emailConfirmationCode));

                var htmlContent = callbackurl.RenderPartialViewToString(RenderPartialView.PartialViewNameChangeLoginAgentUserVerificationEmail);

                var settings = DmsResolver.Current.Get<ISettings>();

                var adminCtx = new AdminContext(ctx);

                var msSetting = new BL.Model.SystemCore.InternalModel.InternalSendMailParameters(
                    new BL.Model.SystemCore.InternalModel.InternalSendMailServerParameters
                    {
                        CheckInterval = settings.GetSetting<int>(adminCtx, SettingConstants.MAIL_TIMEOUT_MIN),
                        ServerType = (MailServerType)settings.GetSetting<int>(adminCtx, SettingConstants.MAIL_SERVER_TYPE),
                        FromAddress = settings.GetSetting<string>(adminCtx, SettingConstants.MAIL_SERVER_SYSTEMMAIL),
                        Login = settings.GetSetting<string>(adminCtx, SettingConstants.MAIL_SERVER_LOGIN),
                        Pass = settings.GetSetting<string>(adminCtx, SettingConstants.MAIL_SERVER_PASS),
                        Server = settings.GetSetting<string>(adminCtx, SettingConstants.MAIL_SERVER_NAME),
                        Port = settings.GetSetting<int>(adminCtx, SettingConstants.MAIL_SERVER_PORT)
                    })
                {
                    Body = htmlContent,
                    ToAddress = model.NewEmail,
                    Subject = "Email confirmation",
                };

                var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
                mailService.SendMessage(ctx, msSetting);
            }

            return new JsonResult(null, this);
        }

        [Route("ConfirmEmailAgentUser")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ConfirmEmailAgentUser(string userId, string code)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var result = await userManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                ApplicationUser user = await userManager.FindByIdAsync(userId);

                if (user == null)
                    throw new UserNameIsNotDefined();

                user.IsEmailConfirmRequired = false;

                result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
            }

            return new JsonResult(null, this);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
