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

namespace DMS_WebAPI.ControllersV3.Employees
{
    /// <summary>
    /// Сотрудники. Паспортные данные
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Employee)]
    public class EmployeePassportController : ApiController
    {
        /// <summary>
        /// Возвращает паспортные данные сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Passport + "/{Id:int}")]
        [ResponseType(typeof(FrontAgentPeoplePassport))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentPeoplePassport(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Корректирует паспортные данные сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Passport)]
        public IHttpActionResult Put([FromBody]ModifyAgentPeoplePassport model)
        {
            Action.Execute(EnumDictionaryActions.ModifyAgentPeoplePassport, model);
            return Get(model.Id);
        }

    }
}
