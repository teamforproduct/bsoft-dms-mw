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

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Адреса юридических лиц
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v2/Companies/Addresses")]
    public class CompaniesAddressesController : ApiController
    {

        /// <summary>
        /// Возвращает список адресов юридической компании
        /// </summary>
        /// <param name="CompanyId">ИД агента</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(List<FrontDictionaryAgentAddress>))]
        public IHttpActionResult Get(int CompanyId, [FromUri] FilterDictionaryAgentAddress filter)
        {
            if (filter == null) filter = new FilterDictionaryAgentAddress();

            if (filter.AgentIDs == null) filter.AgentIDs = new List<int> { CompanyId };
            else filter.AgentIDs.Add(CompanyId);

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentAddresses(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает адрес по ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}")]
        [ResponseType(typeof(FrontDictionaryAgentAddress))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentAddress(ctx, Id);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Создает новый адрес юридической компании
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody]AddDictionaryAgentAddress model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpService.ExecuteAction(EnumDictionaryActions.AddCompanyAddress, ctx, model));
        }

        /// <summary>
        /// Корректирует адрес юридической компании
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public IHttpActionResult Put([FromBody]ModifyDictionaryAgentAddress model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.ModifyCompanyAddress, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет адрес юридической компании
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            tmpService.ExecuteAction(EnumDictionaryActions.DeleteCompanyAddress, ctx, Id);
            FrontDictionaryAgentAddress tmp = new FrontDictionaryAgentAddress();
            tmp.Id = Id;

            return new JsonResult(tmp, this);

        }
    }
}