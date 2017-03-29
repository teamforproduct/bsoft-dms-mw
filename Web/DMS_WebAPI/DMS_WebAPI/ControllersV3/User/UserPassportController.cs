using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Threading.Tasks;
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
        private IHttpActionResult GetById(IContext context)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentPeoplePassport(context, context.CurrentAgentId);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает паспортные данные пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Passport)]
        [ResponseType(typeof(FrontAgentPeoplePassport))]
        public async Task<IHttpActionResult> Get()
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context);
            });
        }

        /// <summary>
        /// Корректирует паспортные данные пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Passport)]
        public async Task<IHttpActionResult> Put([FromBody]AddAgentPeoplePassport model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpModel = new ModifyAgentPeoplePassport()
                {
                    Id = context.CurrentAgentId,
                    PassportDate = model.PassportDate,
                    PassportNumber = model.PassportNumber,
                    PassportSerial = model.PassportSerial,
                    PassportText = model.PassportText
                };
                Action.Execute(context, EnumDictionaryActions.ModifyAgentPeoplePassport, tmpModel);
                return GetById(context);
            });
        }

    }
}