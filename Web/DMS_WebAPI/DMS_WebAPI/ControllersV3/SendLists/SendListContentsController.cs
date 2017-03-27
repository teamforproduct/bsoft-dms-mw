using BL.CrossCutting.DependencyInjection;
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
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.SendLists
{
    /// <summary>
    /// Списки рассылки. Пункты
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.SendList)]
    public class SendListContentsController : ApiController
    {
        /// <summary>
        /// Возвращает список рассылки
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Contents)]
        [ResponseType(typeof(List<FrontDictionaryStandartSendListContent>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterDictionaryStandartSendListContent filter)
        {
            if (filter == null) filter = new FilterDictionaryStandartSendListContent();
            filter.StandartSendListId = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetDictionaryStandartSendListContents(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает пункт списка рассылки по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Contents + "/{Id:int}")]
        [ResponseType(typeof(FrontDictionaryStandartSendListContent))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItem = tmpService.GetDictionaryStandartSendListContent(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Создает новый пункт списка рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Contents)]
        public IHttpActionResult Post([FromBody]AddStandartSendListContent model)
        {
            var tmpItem = Action.Execute(EnumDictionaryActions.AddStandartSendListContent, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует пункт списка рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Contents)]
        public IHttpActionResult Put([FromBody]ModifyStandartSendListContent model)
        {
            Action.Execute(EnumDictionaryActions.ModifyStandartSendListContent, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет пункт списка рассылки
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Contents + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDictionaryActions.DeleteStandartSendListContent, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }
    }
}