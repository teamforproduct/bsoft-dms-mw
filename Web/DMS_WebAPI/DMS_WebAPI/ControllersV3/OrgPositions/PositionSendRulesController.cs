﻿using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;

using BL.Model.Common;
using System.Web;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.DictionaryCore.FrontMainModel;
using System.Diagnostics;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.IncomingModel;

namespace DMS_WebAPI.ControllersV3.OrgPositions
{
    /// <summary>
    /// Правила рассылки документов.
    /// Регулирование правил рассылки документов от отдной должности к другой.
    /// Рассылка бывает двух типов: для исполнения и для сведения.
    /// Этот функционал по умолчанию выключен для нового клиента, рассылка по-умолчанию устанавливается на все должности. 
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Position)]
    public class PositionSendRulesController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список должностей с пычками
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.SendRules)]
        [ResponseType(typeof(List<FrontAdminSubordination>))]
        public IHttpActionResult Get([FromUri] int Id, [FromUri] FilterAdminSubordinationTree filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterAdminSubordinationTree();
            filter.IsChecked = true;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetSubordinationsDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Режим корректировки. Возвращает полный список должностей с пычками для управления рассылкой для сведения и для исполнения.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.SendRules + "/Edit")]
        [ResponseType(typeof(List<FrontAdminSubordination>))]
        public IHttpActionResult GetEdit([FromUri] int Id, [FromUri] FilterAdminSubordinationTree filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterAdminSubordinationTree();
            filter.IsChecked = false;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetSubordinationsDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Разрешает/запрещает должности выполнять рассылку на другую должность с учетом типа рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.SendRules + "/Set")]
        public IHttpActionResult Set([FromBody] ModifyAdminSubordination model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetSubordination, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Разрешает/запрещает должности выполнять рассылку для всех должностей отдела и дочерних отделов с учетом типа рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.SendRules + "/SetByDepartment")]
        public IHttpActionResult SetByDepartment([FromBody] ModifyAdminSubordinationByDepartment model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetSubordinationByDepartment, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Разрешает/запрещает должности выполнять рассылку для всех должностей организации с учетом типа рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.SendRules + "/SetByCompany")]
        public IHttpActionResult SetByCompany([FromBody] ModifyAdminSubordinationByCompany model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetSubordinationByCompany, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Устанавливает рассылку по умолчанию
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.SendRules + "/SetDefault")]
        public IHttpActionResult SetDefault([FromBody] ModifyAdminDefaultByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetDefaultSubordination, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Разрешает/запрещает должности выполнять рассылку для всех должностей организации
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.SendRules + "/SetAll")]
        public IHttpActionResult SetAll([FromBody] ModifyAdminDefaultByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetAllSubordination, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Копирует настроки рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.SendRules + "/Duplicate")]
        public IHttpActionResult Duplicate([FromBody] CopyAdminSettingsByPosition model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.DuplicateSubordinations, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
