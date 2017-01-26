using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Exception;
using BL.Logic.DictionaryCore.Interfaces;
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
using System.Web.Script.Serialization;
using BL.Model.SystemCore.FrontModel;
using BL.Model.WebAPI.Filters;
using System.Linq;
using BL.CrossCutting.Context;
using System.IO;

namespace DMS_WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "Users")]
    public class UsersController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает информацию о пользователе: имя, язык, контакты, адреса
        /// </summary>
        /// <returns></returns>
        [Route("UserInfo")]
        [HttpGet]
        [ResponseType(typeof(FrontDictionaryAgentUser))]
        public IHttpActionResult GetUserInfo()
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var dicProc = DmsResolver.Current.Get<IDictionaryService>();

            var agent = dicProc.GetDictionaryAgentUser(context, context.CurrentAgentId);

            return new JsonResult(agent, this);
        }

        /// <summary>
        /// Получение информации о пользователе
        /// </summary>
        /// <returns></returns>
        [Route("AgentUserInfo")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAgentUserInfo(int agentId)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();

            var webService = new WebAPIService();

            ApplicationUser user = await webService.GetUserAsync(ctx, agentId);
             
            if (user == null) throw new UserIsNotDefined();

            return new JsonResult(new { UserName = user.Email, IsLockout = user.IsLockout, IsEmailConfirmRequired = user.IsEmailConfirmRequired, IsChangePasswordRequired = user.IsChangePasswordRequired, Email = user.Email, EmailConfirmed = user.EmailConfirmed, AccessFailedCount = user.AccessFailedCount, UserId = user.Id }, this);
        }

        /// <summary>
        /// Возвращает список назначений для текущего пользователя (должность - интервал назначения, количество новых событий)
        /// </summary>
        /// <returns>список должностей</returns>
        [Route("AvailablePositions")]
        [HttpGet]
        [ResponseType(typeof(List<FrontUserAssignments>))]
        public IHttpActionResult AvailablePositions()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var context = DmsResolver.Current.Get<UserContexts>().Get(keepAlive:false);
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAvailablePositions(context);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает список Id должностей, от которых пользователь сейчас работатет.
        /// 
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
            var userContexts = DmsResolver.Current.Get<UserContexts>();
            var context = userContexts.Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            tmpService.VerifyAccess(context, new VerifyAccess() { PositionsIdList = positionsIdList });
            userContexts.SetUserPositions(context.CurrentEmployee.Token, positionsIdList);
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

        ///// <summary>
        ///// Установить сервер для использования
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[Route("Servers")]
        //[HttpPost]
        //public async Task<IHttpActionResult> SetServers([FromBody]SetUserServer model)
        //{
        //    var mngContext = DmsResolver.Current.Get<UserContexts>();



        //    var dbProc = new WebAPIDbProcess();

        //    // Получаю первый попавшийся сервер, в который сконфигурен пользователь
        //    var db = dbProc.GetServerByUser(User.Identity.GetUserId(), model);
        //    if (db == null)
        //    {
        //        throw new DatabaseIsNotFound();
        //    }

        //    mngContext.Set(db, model.ClientId);
        //    var ctx = mngContext.Get();

        //    var logger = DmsResolver.Current.Get<ILogger>();
        //    HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
        //    var userAgent = HttpContext.Current.Request.UserAgent;
        //    var mobile = userAgent.Contains("Mobile") ? "Mobile; " : string.Empty;
        //    var ip = HttpContext.Current.Request.Headers["X-Real-IP"];
        //    if (string.IsNullOrEmpty(ip))
        //        ip = HttpContext.Current.Request.UserHostAddress;
        //    var message = $"{ip}; {bc.Browser} {bc.Version}; {bc.Platform}; {mobile}";
        //    //{HttpContext.Current.Request.UserHostAddress}
        //    //var js = new JavaScriptSerializer();
        //    //message += $"; {js.Serialize(HttpContext.Current.Request.Headers)}";
        //    //message += $"; {HttpContext.Current.Request.Headers["X-Real-IP"]}";
        //    var loginLogId = logger.Information(ctx, message, (int)EnumObjects.System, (int)EnumSystemActions.Login);
        //    mngContext.Set(loginLogId, message);
        //    return new JsonResult(null, this);
        //}

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

        ///// <summary>
        ///// Установить клиента для пользователя
        ///// </summary>
        ///// <param name="clientId"></param>
        ///// <returns></returns>
        //[Route("Clients")]
        //[HttpPost]
        //public IHttpActionResult SetClients([FromBody]int clientId)
        //{
        //    var mngContext = DmsResolver.Current.Get<UserContexts>();

        //    var ctx = mngContext.Get();

        //    var dbProc = new WebAPIDbProcess();

        //    var client = dbProc.GetClientByUser(User.Identity.GetUserId(), clientId);
        //    if (client == null)
        //    {
        //        throw new ClientIsNotFound();
        //    }

        //    mngContext.Set(client);


        //    return new JsonResult(null, this);
        //}


        [Route("ConfirmEmailAgentUser")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ConfirmEmailAgentUser(string userId, string code)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var webService = new WebAPIService();

            await webService.ConfirmEmailAgentUser(userId, code);

            return new JsonResult(null, this);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RestorePasswordAgentUser")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RestorePasswordAgentUser(RestorePasswordAgentUser model)
        {
            #region log to file
            try
            {
                string message = "RestorePasswordAgentUser";
                message += DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm") + " UTC\r\n";

                try
                {
                    message += $"URL: {HttpContext.Current.Request.Url.ToString()}\r\n";
                }
                catch { }

                try
                {
                    message += $"Request Body: Email: {model.Email}, ClientCode: {model.ClientCode}\r\n";
                }
                catch { }

                AppendToFile(HttpContext.Current.Server.MapPath("~/RestorePasswordAgentUser.txt"), message);
            }
            catch { }
            #endregion log to file

            var webService = new WebAPIService();
            //new NameValueCollection { { "test", "test" } }
            await webService.RestorePasswordAgentUserAsync(model, new Uri(new Uri(ConfigurationManager.AppSettings["WebSiteUrl"]), "restore-password").ToString(), null, "Restore Password", RenderPartialView.RestorePasswordAgentUserVerificationEmail);

            return new JsonResult(null, this);
        }

        [Route("ConfirmRestorePasswordAgentUser")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ConfirmRestorePasswordAgentUser([FromUri]ConfirmRestorePasswordAgentUser model)
        {
            #region log to file
            try
            {
                string message = "ConfirmRestorePasswordAgentUser";
                message += DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm") + " UTC\r\n";

                try
                {
                    message += $"URL: {HttpContext.Current.Request.Url.ToString()}\r\n";
                }
                catch { }

                try
                {
                    message += $"Request Body: UserId: {model.UserId}, Code: {model.Code}, IsKillSessions: {model.IsKillSessions}, NewPassword: {model.NewPassword}, ConfirmPassword: {model.ConfirmPassword}\r\n";
                }
                catch { }

                AppendToFile(HttpContext.Current.Server.MapPath("~/ConfirmRestorePasswordAgentUser.txt"), message);
            }
            catch { }
            #endregion log to file

            var webService = new WebAPIService();

            var res = await webService.ConfirmRestorePasswordAgentUser(model);

            return new JsonResult(new { UserName = res }, this);
        }

        private static void AppendToFile(string path, string text)
        {
            try
            {
                StreamWriter sw;

                try
                {
                    FileInfo ff = new FileInfo(path);
                    if (ff.Exists)
                    {
                    }
                }
                catch
                {

                }

                sw = File.AppendText(path);
                try
                {
                    string line = text;
                    sw.WriteLine(line);
                }
                catch
                {
                }
                finally
                {
                    sw.Close();
                }
            }
            catch
            {
            }
        }

    }
}
