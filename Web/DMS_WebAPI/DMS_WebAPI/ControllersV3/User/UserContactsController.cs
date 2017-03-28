using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Пользователь. Контакты
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserContactsController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentContact(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список контактов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Contacts)]
        [ResponseType(typeof(List<FrontDictionaryAgentContact>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryContact filter)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                if (filter == null) filter = new FilterDictionaryContact();
                filter.AgentIDs = new List<int> { context.CurrentAgentId };

                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetAgentContacts(context, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает контакт по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Contacts + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentContact))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Создает новый контакт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Contacts)]
        public async Task<IHttpActionResult> Post([FromBody]BaseAgentContact model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var contact = new AddAgentContact(model);
                contact.AgentId = context.CurrentAgentId;
                var tmpItem = Action.Execute(context, EnumDictionaryActions.AddEmployeeContact, contact);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует контакт
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Contacts)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyUserContact model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var contact = new ModifyAgentContact(model);
                contact.AgentId = context.CurrentAgentId;
                Action.Execute(context, EnumDictionaryActions.ModifyEmployeeContact, contact);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет контакт
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Contacts + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                Action.Execute(context, EnumDictionaryActions.DeleteEmployeeContact, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

    }
}