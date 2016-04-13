﻿using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Адреса контрагента
    /// </summary>
    [Authorize]
    public class DictionaryAgentAddressesController : ApiController
    {
       
        /// <summary>
        /// Список всех адресов агента
        /// </summary>
        /// <param name="agentId">ИД агента</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        public IHttpActionResult Get(int agentId,[FromUri] FilterDictionaryAgentAddress filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentAddresses(cxt, agentId,filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Запись из справочника адресов
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentAddress(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление записи справочника адресов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryAgentAddress model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentAddress, cxt, model));
        }

        /// <summary>
        /// Изменение записи справочника адресов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgentAddress model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentAddress, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи справочника адресов
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentAddress, cxt, id);
            FrontDictionaryAgentAddress tmp = new FrontDictionaryAgentAddress();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}