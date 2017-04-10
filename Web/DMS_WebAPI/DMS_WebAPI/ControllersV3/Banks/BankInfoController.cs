﻿using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Banks
{
    /// <summary>
    /// Банки
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Bank)]
    public class BankInfoController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetAgentBank(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Список банков
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <param name="sorting">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontMainAgentBank>))]
        public async Task<IHttpActionResult> GetMain([FromUri]FullTextSearch ftSearch, [FromUri]FilterDictionaryAgentBank filter, [FromUri]UIPaging paging, [FromUri]UISorting sorting)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDictionaryService>();
                var tmpItems = tmpService.GetMainAgentBanks(context, ftSearch, filter, paging, sorting);
                var res = new JsonResult(tmpItems, this);
                res.Paging = paging;
                return res;
            });
        }


        /// <summary>
        /// Возвращает реквизиты банка
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontAgentBank))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет банк
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Post([FromBody]AddAgentBank model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDictionaryActions.AddAgentBank, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует реквизиты банка
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyAgentBank model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.ModifyAgentBank, model);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет банк
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.DeleteAgentBank, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

    }
}
