﻿using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Diagnostics;
using BL.Logic.SystemCore.Interfaces;
using BL.Model.SystemCore.Filters;
using BL.Model.Constants;
using BL.CrossCutting.Interfaces;
using BL.Model.Tree;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Описывает возможности должностей просматривать и регистрировать документы в журналах регистрации
    /// По умолчанию должность может просматривать и регистрировать документы во всех журналах своего отдела 
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/AdminRegistrationJournalPositions")]
    public class AdminRegistrationJournalPositionsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();


        /// <summary>
        /// Устанавливает доступ для должности к журналу регистрации на просмотр или на регистрацию
        /// </summary>
        /// <param name="model">ModifyAdminRegistrationJournalPosition</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Set")]
        public IHttpActionResult Set([FromBody] ModifyAdminRegistrationJournalPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetRegistrationJournalPosition, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности к журналам регистрации в указанном отделе и дочерних
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetByDepartment")]
        public IHttpActionResult SetByDepartment([FromBody] ModifyAdminRegistrationJournalPositionByDepartment model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetRegistrationJournalPositionByDepartment, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности к журналам регистрации в для всех отделов указанной компании
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetByCompany")]
        public IHttpActionResult SetByCompany([FromBody] ModifyAdminRegistrationJournalPositionByCompany model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetRegistrationJournalPositionByCompany, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности к журналам регистрации в своем отделе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetDefault")]
        public IHttpActionResult SetDefault([FromBody] ModifyAdminDefaultByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetDefaultRegistrationJournalPosition, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает доступ для должности ко всем журналам регистрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetAll")]
        public IHttpActionResult SetAll([FromBody] ModifyAdminRegistrationJournalPositions model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.SetAllRegistrationJournalPosition, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает список журналов регистрации с пычками для установки доступа на просмотр или регистрацию
        /// </summary>
        /// <param name="positionId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDIP")]
        public IHttpActionResult GetDIP([FromUri] int positionId, [FromUri] FilterTree filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetRegistrationJournalPositionsDIP(ctx, positionId, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Копирует настроки доступов к журналам регистрации от Source к Target должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Duplicate")]
        public IHttpActionResult Duplicate([FromBody] CopyAdminSettingsByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.DuplicateRegistrationJournalPositions, cxt, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: устанавливать доступ на просмотр к журналам регистрации своего отдела при создании новой должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("IsSetAllForViewing")] //IsSetDefaultsForExecution
        public IHttpActionResult IsSetAllForViewing()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetRJournalPositionSetAllForViewing(cxt);//, new FilterSystemSetting() { Key = SettingConstants.RegistrationJournalPositionS_SEND_ALL_FOR_EXECUTION });
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает значение настройки: устанавливать доступ на регистрацию к журналам регистрации своего отдела при создании новой должности
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("IsSetAllForRegistration")]
        public IHttpActionResult IsSetAllForRegistration()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ISettings>();
            var tmpItem = tmpService.GetRJournalPositionSetAllForRegistration(cxt);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}