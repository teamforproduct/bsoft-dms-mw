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
    /// Пользователь. Адреса
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserAddressesController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentAddress(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список адресов
        /// </summary>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Addresses)]
        [ResponseType(typeof(List<FrontDictionaryAgentAddress>))]
        public async Task<IHttpActionResult> Get([FromUri] FilterDictionaryAgentAddress filter)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                if (filter == null) filter = new FilterDictionaryAgentAddress();
                filter.AgentIDs = new List<int> { context.CurrentAgentId };

                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetAgentAddresses(context, filter);
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
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Создает новый адрес
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Addresses)]
        public async Task<IHttpActionResult> Post([FromBody]BaseAgentAddress model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var address = new AddAgentAddress(model);
                address.AgentId = context.CurrentAgentId;
                var tmpItem = Action.Execute(context, EnumDictionaryActions.AddEmployeeAddress, address);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует адрес пользователя-сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Addresses)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyUserAddress model)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                var address = new ModifyAgentAddress(model);
                address.AgentId = context.CurrentAgentId;
                Action.Execute(context, EnumDictionaryActions.ModifyEmployeeAddress, address);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет адрес пользователя-сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Addresses + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                Action.Execute(context, EnumDictionaryActions.DeleteEmployeeAddress, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }
    }
}