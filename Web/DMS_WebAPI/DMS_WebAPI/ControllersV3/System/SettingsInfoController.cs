﻿using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.SystemCore.Filters;
using BL.Model.SystemCore.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// Системные настройки
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Settings)]
    public class SettingsInfoController : ApiController
    {
        /// <summary>
        /// Список настроек
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info)]
        [ResponseType(typeof(FrontDictionarySettingType))]
        public IHttpActionResult Get([FromUri] FilterSystemSetting filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISystemService>();
            var tmpItems = tmpService.GetSystemSettings(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }


        /// <summary>
        /// Устанавливает значение настойки
        /// </summary>
        /// <param name="model">ModifyAdminPositionRole</param>
        /// <returns>FrontAdminPositionRole</returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]List<ModifySystemSetting> model)
        {
            var tmpItem = Action.Execute(EnumSystemActions.SetSetting, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}