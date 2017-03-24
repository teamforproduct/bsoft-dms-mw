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
using System.Threading.Tasks;
using BL.CrossCutting.Interfaces;
using BL.Model.SystemCore;

namespace DMS_WebAPI.ControllersV3.Banks
{
    /// <summary>
    /// Банки. Контакты
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Bank)]
    public class BankContactsController : ApiController
    {
        private IHttpActionResult GetById(IContext ctx, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentContact(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список контактов
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Contacts)]
        [ResponseType(typeof(List<FrontDictionaryAgentContact>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterDictionaryContact filter)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                if (filter == null) filter = new FilterDictionaryContact();
                filter.AgentIDs = new List<int> { Id };
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
        public async Task<IHttpActionResult> Post([FromBody]AddAgentContact model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var tmpItem = Action.Execute(EnumDictionaryActions.AddBankContact, model);
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
        public async Task<IHttpActionResult> Put([FromBody]ModifyAgentContact model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                Action.Execute(EnumDictionaryActions.ModifyBankContact, model);
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
                Action.Execute(EnumDictionaryActions.DeleteBankContact, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }
    }
}