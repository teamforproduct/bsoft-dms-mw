﻿using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.Users;
using DMS_WebAPI.Models;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Auth
{
    /// <summary>
    /// Восстановление авторизациии сотрудников-пользователей
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Auth)]
    public class AuthRestoreController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        public class ConfirmEmail
        {
            public string userId { get; set; }

            public string code { get; set; }
        }

        /// <summary>
        /// Подтверждает адрес пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IHttpActionResult> SetConfirmEmail([FromBody] ConfirmEmail model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = new WebAPIService();
            await webService.ConfirmEmailAgentUser(model.userId, model.code);

            var res = new JsonResult(null, this);
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
        public async Task<IHttpActionResult> RestorePassword(RestorePasswordAgentUser model)
        {
            var webService = new WebAPIService();
            await webService.RestorePasswordAgentUserAsync(model, new Uri(new Uri(ConfigurationManager.AppSettings["WebSiteUrl"]), "restore-password").ToString(), null, "Restore Password", RenderPartialView.RestorePasswordAgentUserVerificationEmail);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Подтверждает пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("ConfirmRestorePassword")]
        public async Task<IHttpActionResult> ConfirmRestorePasswordAgentUser([FromBody]ConfirmRestorePasswordAgentUser model)
        {
            var webService = new WebAPIService();
            var res = await webService.ConfirmRestorePasswordAgentUser(model);
            return new JsonResult(new { UserName = res }, this);
        }
    }
}