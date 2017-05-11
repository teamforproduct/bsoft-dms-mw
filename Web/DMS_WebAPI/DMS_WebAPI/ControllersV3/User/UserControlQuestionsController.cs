using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;
using BL.Model.WebAPI.FrontModel;
using BL.Model.WebAPI.IncomingModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Контрольный вопрос и ответ
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserControlQuestionController : WebApiController
    {
        private IHttpActionResult GetById(IContext context)
        {
            var webService = DmsResolver.Current.Get<WebAPIService>();
            var user = webService.GetUserById(context.User.Id);
            var res = new JsonResult(new FrontAspNetUserControlQuestion
            {
                Question = user.ControlQuestion?.Name,
                QuestionId = user.ControlQuestionId ?? -1,
                Answer = user.ControlAnswer
            }, this);
            return res;
        }

        /// <summary>
        /// Возвращает контрольный вопрос и ответ
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ControlQuestion)]
        public IHttpActionResult Get()
        {
            //!ASYNC
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            return GetById(context);
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
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var webService = DmsResolver.Current.Get<WebAPIService>();
            webService.ChangeControlQuestion(model);
            return GetById(context);
        }

    }
}