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

namespace DMS_WebAPI.ControllersV3.User
{
    /// <summary>
    /// Списки рассылки
    /// </summary>
    [Authorize]
    //![DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.User)]
    public class UserSendListController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetUserStandartSendList(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }


        /// <summary>
        /// Список заголовков списков рассылки
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SendLists + "/Main")]
        [ResponseType(typeof(List<FrontMainDictionaryStandartSendList>))]
        public IHttpActionResult GetMain([FromUri]FullTextSearch ftSearch, [FromUri]FilterDictionaryStandartSendList filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetMainUserStandartSendLists(ctx, ftSearch, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает заголовок списка рассылки
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.SendLists + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryStandartSendList))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет заголовок списка рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.SendLists)]
        public async Task<IHttpActionResult> Post([FromBody]AddStandartSendList model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDictionaryActions.AddStandartSendList, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует заголовок списка рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.SendLists)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyStandartSendList model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.ModifyStandartSendList, model);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет заголовок списка рассылки
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.SendLists + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDictionaryActions.DeleteStandartSendList, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

    }
}
