using BL.Logic.DictionaryCore.Interfaces;
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
using BL.Model.Common;
using System.Diagnostics;
using BL.Model.SystemCore;

namespace DMS_WebAPI.ControllersV3.Companies
{
    /// <summary>
    /// Юридические лица. Контакты контактного лица
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Company)]
    public class CompanyContactPersonsContactsController : ApiController
    {
        /// <summary>
        /// Возвращает список контактов
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.ContactPersons + "/{Id:int}/Contacts")]
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
        [Route(Features.ContactPersons + "/Contacts/{Id:int}")]
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
        [Route(Features.ContactPersons + "/Contacts")]
        public IHttpActionResult Post([FromBody]AddAgentContact model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddEmployeeContact, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует контакт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.ContactPersons + "/Contacts")]
        public IHttpActionResult Put([FromBody]ModifyAgentContact model)
        {
            Action.Execute(EnumDictionaryActions.ModifyEmployeeContact, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет контакт
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.ContactPersons + "/Contacts/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteEmployeeContact, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }
    }
}