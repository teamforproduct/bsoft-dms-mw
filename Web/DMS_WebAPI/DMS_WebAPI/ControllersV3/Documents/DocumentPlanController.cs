using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Планы.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentPlanController : ApiController
    {
        /// <summary>
        /// Возвращает список пунктов плана по ИД документа
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/"+Features.Plan)]
        [ResponseType(typeof(List<FrontDocumentSendList>))]
        public IHttpActionResult GetByDocumentId(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            var items = docProc.GetSendLists(ctx, Id);
            var res = new JsonResult(items, this);
            return res;
        }

        /// <summary>
        /// Возвращает пунктов плана по ИД
        /// </summary>
        /// <param name="Id">ИД пункта плана</param>
        /// <returns>запись пункта плана</returns>
        [HttpGet]
        [Route(Features.Plan + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentSendList))]
        public IHttpActionResult GetById(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            var item = docProc.GetSendList(ctx, Id);
            var res = new JsonResult(item, this);
            return res;
        }

        /// <summary>
        /// Добавляет пункт плана
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Plan)]
        public IHttpActionResult Post([FromBody]AddDocumentSendList model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.AddDocumentSendList, model, model.CurrentPositionId);
            //var res = new JsonResult(tmpItem, this);
            return GetById(tmpItem);
        }

        /// <summary>
        /// Добавляет этап плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Plan+"/AddStage")]
        public IHttpActionResult AddStage([FromBody]ModifyDocumentSendListStage model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = (bool)docProc.ExecuteAction(EnumDocumentActions.AddDocumentSendListStage, ctx, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Измененяет пункт плана
        /// </summary>
        /// <param name="model">Модель для обновления пункта плана</param>
        /// <returns>Обновленный пункт плана</returns>
        [HttpPut]
        [Route(Features.Plan)]
        public IHttpActionResult Put([FromBody]ModifyDocumentSendList model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.ModifyDocumentSendList, model);
            //var res = new JsonResult(tmpItem, this);
            return GetById(model.Id);
        }

        /// <summary>
        /// Удаляет пункт плана
        /// </summary>
        /// <param name="Id">ИД пункта пдана</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Plan + "/{Id:int}")]
        public IHttpActionResult Delete(int Id)
        {
            Action.Execute(EnumDocumentActions.DeleteDocumentSendList, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Удаляет этапа плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Plan + "/DeleteStage")]
        public IHttpActionResult DeleteStage([FromUri]ModifyDocumentSendListStage model)
        {
            Action.Execute(EnumDocumentActions.DeleteDocumentSendListStage, model);
            var res = new JsonResult(null, this);
            return res;
        }
        /// <summary>
        /// Возвращает меню по ИД документа для работы с планами 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Plan+ "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public IHttpActionResult Actions([FromUri]int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<ICommandService>();
            var items = docProc.GetDocumentSendListActions(ctx, Id);
            var res = new JsonResult(items, this);
            return res;
        }

        /// <summary>
        /// Возвращает список досылки для связанных документов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Plan + "/AdditinalLinkedDocument")]
        [ResponseType(typeof(List<FrontDocument>))]
        public IHttpActionResult AdditinalLinkedDocumentSendLists([FromBody]AdditinalLinkedDocumentSendList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            var items = docProc.GetAdditinalLinkedDocumentSendLists(ctx, model);
            var res = new JsonResult(items, this);
            return res;
        }

        /// <summary>
        /// Принудительно запускает пункт плана на исполнение
        /// </summary>
        /// <param name="model">ИД пункта плана</param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Plan+ "/LaunchItem")]
        public IHttpActionResult LaunchItem([FromBody]Item model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var aplan = DmsResolver.Current.Get<IAutoPlanService>();
            aplan.ManualRunAutoPlan(ctx, model.Id, null);
            var res = new JsonResult(model.Id, this);
            return res;
        }

        /// <summary>
        /// Запускает автоматическую отработку плана
        /// </summary>
        /// <param name="model">ИД документа</param>
        /// <returns></returns>
        [Route(Features.Plan + "/LaunchPlan")]
        [HttpPut]
        public IHttpActionResult LaunchPlan([FromBody]Item model)
        {
            Action.Execute(EnumDocumentActions.LaunchPlan, model.Id);
            var res = new JsonResult(null, this);
            return res;
        }

        /// <summary>
        /// Останавливет автоматическую отработку плана
        /// </summary>
        /// <param name="model">ИД документа</param>
        /// <returns></returns>
        [Route(Features.Plan + "/StopPlan")]
        [HttpPut]
        public IHttpActionResult StopPlan([FromBody]Item model)
        {
            Action.Execute(EnumDocumentActions.StopPlan, model.Id);
            var res = new JsonResult(null, this);
            return res;
        }

    }
}
