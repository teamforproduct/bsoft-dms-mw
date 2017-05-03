﻿using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.DBModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.Auth
{
    /// <summary>
    /// Авторизация. Контрольные вопросы
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Auth)]
    public class AuthControlQuestionController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();


        /// <summary>
        /// Возвращает контрольные вопросы
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("ControlQuestions")]
        public IHttpActionResult Get(string language)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = DmsResolver.Current.Get<WebAPIService>();
            var tmpItems = webService.GetControlQuestions(language);

            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Возвращает контрольный вопрос пользователя
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("ControlQuestion")]
        public async Task<IHttpActionResult> Get([FromBody] UserAuth model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var webService = DmsResolver.Current.Get<WebAPIService>();

            AspNetUsers user = await webService.GetUser(model.UserName, model.Password);

            if (user == null) throw new UserNameOrPasswordIsIncorrect();

            var tmpService = DmsResolver.Current.Get<ILanguages>();
            var qst = tmpService.GetTranslation(model.Language, user.ControlQuestion.Name);

            var res = new JsonResult(new { ControlQuestion = qst }, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
