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
    /// Адреса физических лиц
    /// </summary>
    [Authorize]
    public class PersonAddressesController : ApiController
    {

        /// <summary>
        /// Возвращает список адресов физического лица
        /// </summary>
        /// <param name="id">ИД агента</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [ResponseType(typeof(List<FrontDictionaryAgentAddress>))]
        public IHttpActionResult Get(int id, [FromUri] FilterDictionaryAgentAddress filter)
        {
            if (filter == null) filter = new FilterDictionaryAgentAddress();

            if (filter.AgentIDs == null) filter.IDs = new List<int> { id };
            else filter.AgentIDs.Add(id);

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryAgentAddresses(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// Возвращает адрес по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(FrontDictionaryAgentAddress))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryAgentAddress(ctx, id);
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Создает новый адрес физического лица
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryAgentAddress model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpService.ExecuteAction(EnumDictionaryActions.AddPersonAddress, ctx, model));
        }

        /// <summary>
        /// Корректирует адрес физического лица
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgentAddress model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            tmpService.ExecuteAction(EnumDictionaryActions.ModifyPersonAddress, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет адрес физического лица
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();

            tmpService.ExecuteAction(EnumDictionaryActions.DeletePersonAddress, ctx, id);
            FrontDictionaryAgentAddress tmp = new FrontDictionaryAgentAddress();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}