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

namespace DMS_WebAPI.ControllersV3.Companies
{
    /// <summary>
    /// Юридические лица. Расчетные счета
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Company)]
    public class CompanyAccountsController : ApiController
    {

        /// <summary>
        /// Возвращает список расчетных счетов
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Accounts)]
        [ResponseType(typeof(List<FrontDictionaryAgentAccount>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterDictionaryAgentAccount filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return await this.SafeExecuteAsync(ModelState, ctx, context => { 
                if (filter == null) filter = new FilterDictionaryAgentAccount();
                filter.AgentIDs = new List<int> { Id };
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetDictionaryAgentAccounts(ctx, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает расчетный счет по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Accounts + "/{Id:int}")]
        [ResponseType(typeof (FrontDictionaryAgentAccount))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return await this.SafeExecuteAsync(ModelState, ctx, context =>
            {
                return Get(ctx, Id);
            });
        }

        public IHttpActionResult Get(IContext ctx, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryAgentAccount(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Создает новый расчетный счет
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Accounts)]
        public async Task<IHttpActionResult> Post([FromBody] AddAgentAddress model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return await this.SafeExecuteAsync(ModelState, ctx, context =>
            {
                var tmpItem = Action.Execute(EnumDictionaryActions.AddAgentAccount, model);
                return Get(context,tmpItem);
            });
        }

        /// <summary>
        /// Корректирует расчетный счет
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Accounts)]
        public async Task<IHttpActionResult> Put([FromBody] ModifyAgentAddress model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return await this.SafeExecuteAsync(ModelState, ctx, context =>
            {
                Action.Execute(EnumDictionaryActions.ModifyAgentAccount, model);
                return Get(context,model.Id);
            });
        }

        /// <summary>
        /// Удаляет расчетный счет
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Accounts + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return await this.SafeExecuteAsync(ModelState, ctx, context =>
            {
                Action.Execute(EnumDictionaryActions.DeleteAgentAccount, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }
    }
}