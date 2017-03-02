using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Паспортные данные
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
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