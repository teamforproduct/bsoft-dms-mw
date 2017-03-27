using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Org
{
    /// <summary>
    /// Организации - клиентские компании. Контакты
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Org)]
    public class OrgContactsController : ApiController
    {
        /// <summary>
        /// Возвращает список контактов
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Contacts)]
        [ResponseType(typeof(List<FrontDictionaryAgentContact>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterDictionaryContact filter)
        {
            if (filter == null) filter = new FilterDictionaryContact();
            filter.AgentIDs = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentContacts(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает контакт по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Contacts+ "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentContact))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentContact(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Создает новый контакт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Contacts)]
        public IHttpActionResult Post([FromBody]AddAgentContact model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddClientCompanyContact, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует контакт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Contacts)]
        public IHttpActionResult Put([FromBody]ModifyAgentContact model)
        {
            Action.Execute(EnumDictionaryActions.ModifyClientCompanyContact, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет контакт
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Contacts+ "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteClientCompanyContact, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;

        }
    }
}