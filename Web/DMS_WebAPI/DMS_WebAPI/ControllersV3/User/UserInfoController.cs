using BL.CrossCutting.Context;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Common;
using BL.Model.Database;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.FrontModel;
using BL.Model.Users;
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Контекст пользователя (Все пользователя являются сотрудниками, но у сотрудника может быть выключена возможность авторизации)
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + "User")]
    public class UserInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();


        /// <summary>
        /// Возвращает информацию о пользователе: имя, логин, язык, табельный номер, инн, паспортные данные, дата рождения, пол
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Info")]
        [ResponseType(typeof(FrontDictionaryAgentEmployeeUser))]
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
        [Route("Info/Permissions")]
        [ResponseType(typeof(List<FrontPermission>))]
        public IHttpActionResult GetPermissions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Возвращает историю подключений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/AuthLog")]
        [ResponseType(typeof(List<FrontSystemSession>))]
        public IHttpActionResult GetAuthLog()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Устанавливает новый логин
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info/ChangeLogin")]
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
        [Route("Info/ChangePassword")]
        public IHttpActionResult ChangePassword([FromBody]ChangePassword model)
        {
            throw new NotImplementedException();
        }



    }
}