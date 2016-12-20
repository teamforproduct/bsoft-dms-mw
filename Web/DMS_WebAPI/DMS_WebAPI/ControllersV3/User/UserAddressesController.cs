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
using System.Diagnostics;
using BL.Model.Common;

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Адреса пользователя-сотрудника
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + "User")]
    public class UserAddressesController : ApiController
    {

        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список адресов
        /// </summary>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Addresses")]
        [ResponseType(typeof(List<FrontDictionaryAgentAddress>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentAddress filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();

            if (filter == null) filter = new FilterDictionaryAgentAddress();
            filter.AgentIDs = new List<int> { ctx.CurrentAgentId };

            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentAddresses(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает адрес по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Addresses/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentAddress))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentAddress(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Создает новый адрес
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Addresses")]
        public IHttpActionResult Post([FromBody]AddAgentAddress model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = (int)tmpService.ExecuteAction(EnumDictionaryActions.AddEmployeeAddress, ctx, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует адрес пользователя-сотрудника
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Addresses")]
        public IHttpActionResult Put([FromBody]ModifyAgentAddress model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.ModifyEmployeeAddress, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет адрес пользователя-сотрудника
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Addresses/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.DeleteEmployeeAddress, ctx, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;

        }
    }
}