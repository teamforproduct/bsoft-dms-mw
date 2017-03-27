﻿using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Diagnostics;
using BL.Model.Common;
using BL.Model.SystemCore;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using System.Threading.Tasks;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Контрольный вопрос и ответ
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserControlQuestionController : ApiController
    {

        /// <summary>
        /// Возвращает контрольный вопрос и ответ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ControlQuestion)]
        public IHttpActionResult Get()
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var user = webService.GetUser(ctx, ctx.CurrentAgentId);
            var res = new JsonResult(new FrontAspNetUserControlQuestion
            {
                Question = user.ControlQuestion?.Name,
                QuestionId = user.ControlQuestionId ?? -1,
                Answer = user.ControlAnswer
            }, this);
            return res;
        }

        /// <summary>
        /// Корректирует контрольный вопрос и ответ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ControlQuestion)]
        public IHttpActionResult Put([FromBody]ModifyAspNetUserControlQuestion model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            webService.ChangeControlQuestion(model);
            return Get();
        }

    }
}