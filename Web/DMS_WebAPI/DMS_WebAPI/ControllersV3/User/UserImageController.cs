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
    /// Аватарка пользователя
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.User)]
    public class UserImageController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();


        /// <summary>
        /// Возвращает аватарку
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Image")]
        [ResponseType(typeof(FrontDictionaryAgentEmployeeUser))]
        public IHttpActionResult Get()
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryAgentUserPicture(ctx, ctx.CurrentAgentId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает новую аватарку
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Image")]
        public IHttpActionResult Put([FromUri]ModifyDictionaryAgentImage model)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Удаляет аватарку
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Image/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            throw new NotImplementedException();
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }



    }
}