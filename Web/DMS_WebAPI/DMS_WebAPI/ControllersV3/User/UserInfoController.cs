﻿using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.SystemCore;
using BL.Model.Users;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ChangePassword)]
        public IHttpActionResult ChangePassword([FromBody]ChangePassword model)
        {
            throw new NotImplementedException();
        }



    }
}