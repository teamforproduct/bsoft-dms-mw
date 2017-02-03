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

    }
}
