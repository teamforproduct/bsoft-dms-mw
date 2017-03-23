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
    /// Юридические лица. Адреса
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Company)]
    public class CompanyAddressesController : ApiController
    {
        /// <summary>
        /// Возвращает список адресов
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Addresses)]
        [ResponseType(typeof(List<FrontDictionaryAgentAddress>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterDictionaryAgentAddress filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return await this.SafeExecuteAsync(ModelState, ctx, context =>
            {
                if (filter == null) filter = new FilterDictionaryAgentAddress();
                filter.AgentIDs = new List<int> { Id };

                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetAgentAddresses(ctx, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает адрес по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Addresses + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentAddress))]
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
            var tmpItem = tmpService.GetAgentAddress(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Создает новый адрес
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Addresses)]
        public async Task<IHttpActionResult> Post([FromBody]AddAgentAddress model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return await this.SafeExecuteAsync(ModelState, ctx, context =>
            {
                var tmpItem = Action.Execute(EnumDictionaryActions.AddCompanyAddress, model);
                return Get(ctx, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует адрес
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Addresses)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyAgentAddress model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return await this.SafeExecuteAsync(ModelState, ctx, context =>
            {
                Action.Execute(EnumDictionaryActions.ModifyCompanyAddress, model);
                return Get(ctx, model.Id);

            });
        }

        /// <summary>
        /// Удаляет адрес
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Addresses + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            return await this.SafeExecuteAsync(ModelState, ctx, context =>
            {
                Action.Execute(EnumDictionaryActions.DeleteCompanyAddress, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }
    }
}