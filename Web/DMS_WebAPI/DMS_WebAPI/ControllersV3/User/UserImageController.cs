using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// !!! Доступ не ограничен
    /// Аватарка пользователя
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserImageController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();


        /// <summary>
        /// !!! Доступ не ограничен.
        /// Возвращает аватарку
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Image)]
        [ResponseType(typeof(FrontAgentEmployeeUser))]
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
        [Route(Features.Image)]
        public IHttpActionResult Put([FromBody]ModifyDictionaryAgentImage model)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Удаляет аватарку
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Image + "/{Id:int}")]
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