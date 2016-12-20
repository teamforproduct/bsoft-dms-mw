﻿using BL.CrossCutting.Context;
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
using BL.Model.WebAPI.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Назначения пользователя-сотрудника
    /// Пользователь-сотрудник может работать в системе если назначен хотя бы на одну должность в текущем интервале времени
    /// Пользователь-сотрудник может одновременно занимать должность и исполнять обязанноти другой должности или реферировать
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + "User")]
    public class UserAssignmentsController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список назначений для текущего пользователя (должность - интервал назначения, количество новых событий)
        /// Пользоателю может быть назначено исполнение обязанностей другой должности
        /// </summary>
        /// <returns>список должностей</returns>
        [HttpGet]
        [Route("Assignments/Current")]
        [ResponseType(typeof(List<FrontUserAssignments>))]
        public IHttpActionResult Assignments()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var context = DmsResolver.Current.Get<UserContexts>().Get(keepAlive: false);
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAvailablePositions(context);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает список назначений, от которых пользователь сейчас работатет.
        /// </summary>
        /// <returns>массива ИД должностей</returns>
        [HttpGet]
        [Route("Assignments")]
        [ResponseType(typeof(List<FrontUserAssignments>))]
        public IHttpActionResult GetAssignments()
        {
            //var context = DmsResolver.Current.Get<UserContexts>().Get();
            //return new JsonResult(, this);

            if (!stopWatch.IsRunning) stopWatch.Restart();
            var context = DmsResolver.Current.Get<UserContexts>().Get(keepAlive: false);
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAvailablePositions(context, context.CurrentPositionsIdList);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает назначения из списка доступных.
        /// Пользователь может переключаться между доступными назначениями для разделения информационного потока или выполнять обязанности от всех назначений. 
        /// </summary>
        /// <param name="positionsIdList">ИД должности</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Assignments")]
        public IHttpActionResult Assignments([FromBody]List<int> positionsIdList)
        {
            var userContexts = DmsResolver.Current.Get<UserContexts>();
            var context = userContexts.Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            tmpService.VerifyAccess(context, new VerifyAccess() { PositionsIdList = positionsIdList });

            //TODO Здесь необходима проверка на то, что список должностей из доступных
            userContexts.SetUserPositions(context.CurrentEmployee.Token, positionsIdList);
            //context.CurrentPositionsIdList = positionsIdList;
            //ctx.CurrentPositions = new List<CurrentPosition>() { new CurrentPosition { CurrentPositionId = positionId } };
            return new JsonResult(null, this);
        }

    }
}