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

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Паспортные данные пользователя
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserPassportController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает паспортные данные пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Passport)]
        [ResponseType(typeof(FrontAgentPeoplePassport))]
        public IHttpActionResult Get()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentPeoplePassport(ctx, ctx.CurrentAgentId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Корректирует паспортные данные пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Passport)]
        public IHttpActionResult Put([FromBody]AddAgentPeoplePassport model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpModel = new ModifyAgentPeoplePassport()
            {
                Id = ctx.CurrentAgentId,
                PassportDate = model.PassportDate,
                PassportNumber = model.PassportNumber,
                PassportSerial = model.PassportSerial,
                PassportText = model.PassportText
            };
            Action.Execute(EnumDictionaryActions.ModifyAgentPeoplePassport, tmpModel);
            return Get();
        }

    }
}