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
using System;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Адреса агента (сотрудника, пользователя, банка, юр.лиц и физ.лиц)
    /// </summary>
    [Authorize]
    public class DictionaryAgentAddressesController : ApiController
    {

        /// <summary>
        /// Возвращает список адресов агента
        /// </summary>
        /// <param name="agentId">ИД агента</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [Obsolete()]
        [ResponseType(typeof(List<FrontDictionaryAgentAddress>))]
        public IHttpActionResult Get(int agentId, [FromUri] FilterDictionaryAgentAddress filter)
        {
            if (filter == null) filter = new FilterDictionaryAgentAddress();

            if (filter.AgentIDs == null) filter.AgentIDs = new List<int> { agentId };
            else filter.AgentIDs.Add(agentId);

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetAgentAddresses(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает адрес агента по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Obsolete()]
        [ResponseType(typeof(FrontDictionaryAgentAddress))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetAgentAddress(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление записи справочника адресов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Obsolete()]
        public IHttpActionResult Post([FromBody]AddAgentAddress model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentAddress, ctx, model));
        }

        /// <summary>
        /// Изменение записи справочника адресов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Obsolete()]
        public IHttpActionResult Put(int id, [FromBody]ModifyAgentAddress model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentAddress, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи справочника адресов
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Obsolete()]
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentAddress, ctx, id);
            FrontDictionaryAgentAddress tmp = new FrontDictionaryAgentAddress();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}
