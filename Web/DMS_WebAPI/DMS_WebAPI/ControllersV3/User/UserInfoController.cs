﻿using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.SystemCore;
using BL.Model.Users;
using DMS_WebAPI.Models;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// !!! Доступ не ограничен. Информация о себе всегда доступна
    /// Контекст пользователя (Все пользователя являются сотрудниками, но у сотрудника может быть выключена возможность авторизации)
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserInfoController : ApiController
    {

        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public UserInfoController()
        {
        }

        public UserInfoController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        Stopwatch stopWatch = new Stopwatch();


        /// <summary>
        /// Возвращает информацию о пользователе: имя, логин, язык, табельный номер, инн, дата рождения, пол
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info)]
        [ResponseType(typeof(FrontAgentEmployeeUser))]
        public IHttpActionResult Get()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = new WebAPIService();
            var tmpItem = webService.GetUserInfo(ctx);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает набор прав в терминах: module, feature, CRUD
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Permissions)]
        [ResponseType(typeof(List<FrontPermission>))]
        public IHttpActionResult GetPermissions()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetUserPermissions(ctx);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает историю подключений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.AuthLog)]
        [ResponseType(typeof(List<FrontSystemSession>))]
        public IHttpActionResult GetAuthLog()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Устанавливает новый логин
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ChangeLogin)]
        public IHttpActionResult ChangeLogin([FromBody]ChangeLogin model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Устанавливает новый пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return new JsonResult(result, false, string.Join(" ", result.Errors), this);
            }

            return Ok();
        }

        /// <summary>
        /// Изменяет текущий пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ChangePassword)]
        public async Task<IHttpActionResult> ChangePassword([FromBody]ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(ModelState, false, this);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return new JsonResult(result, false, string.Join(" ", result.Errors), this);
            }

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser user = await userManager.FindByIdAsync(User.Identity.GetUserId());

            user.IsChangePasswordRequired = false;

            result = await UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new JsonResult(result, false, string.Join(" ", result.Errors), this);
            }

            var user_context = DmsResolver.Current.Get<UserContexts>();
            user_context.UpdateChangePasswordRequired(user.Id, false);

            return new JsonResult(null, this);
        }

        /// <summary>
        /// Выход
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            DmsResolver.Current.Get<UserContexts>().Remove();

            Request.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return new JsonResult(null, this);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

    }
}