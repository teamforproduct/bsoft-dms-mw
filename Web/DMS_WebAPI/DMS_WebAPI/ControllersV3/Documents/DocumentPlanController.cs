using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
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
using System.Threading.Tasks;
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
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            var item = docProc.GetSendList(context, Id);
            var res = new JsonResult(item, this);
            return res;
        }

        /// <summary>
        /// Возвращает список пунктов плана по ИД документа
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Plan)]
        [ResponseType(typeof(List<FrontDocumentSendList>))]
        public async Task<IHttpActionResult> GetByDocumentId(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
                   var items = docProc.GetSendLists(context, Id);
                   var res = new JsonResult(items, this);
                   return res;
               });
        }

        /// <summary>
        /// Возвращает пунктов плана по ИД
        /// </summary>
        /// <param name="Id">ИД пункта плана</param>
        /// <returns>запись пункта плана</returns>
        [HttpGet]
        [Route(Features.Plan + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentSendList))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет пункт плана
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Plan)]
        public async Task<IHttpActionResult> Post([FromBody]AddDocumentSendList model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.Execute(context, EnumDocumentActions.AddDocumentSendList, model, model.CurrentPositionId);
                   //var res = new JsonResult(tmpItem, this);
                   return GetById(context, tmpItem);
               });
        }

        /// <summary>
        /// Добавляет этап плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Plan + "/AddStage")]
        public async Task<IHttpActionResult> AddStage([FromBody]ModifyDocumentSendListStage model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentService>();
                   var tmpItem = (bool)docProc.ExecuteAction(EnumDocumentActions.AddDocumentSendListStage, context, model);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Измененяет пункт плана
        /// </summary>
        /// <param name="model">Модель для обновления пункта плана</param>
        /// <returns>Обновленный пункт плана</returns>
        [HttpPut]
        [Route(Features.Plan)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyDocumentSendList model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.Execute(context, EnumDocumentActions.ModifyDocumentSendList, model);
                   //var res = new JsonResult(tmpItem, this);
                   return GetById(context, model.Id);
               });
        }

        /// <summary>
        /// Удаляет пункт плана
        /// </summary>
        /// <param name="Id">ИД пункта пдана</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Plan + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.DeleteDocumentSendList, Id);
                   var tmpItem = new FrontDeleteModel(Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Удаляет этапа плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Plan + "/DeleteStage")]
        public async Task<IHttpActionResult> DeleteStage([FromUri]ModifyDocumentSendListStage model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.DeleteDocumentSendListStage, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }
        /// <summary>
        /// Возвращает меню по ИД документа для работы с планами 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Plan + "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public async Task<IHttpActionResult> Actions([FromUri]int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<ICommandService>();
                   var items = docProc.GetDocumentSendListActions(context, Id);
                   var res = new JsonResult(items, this);
                   return res;
               });
        }

        /// <summary>
        /// Возвращает список досылки для связанных документов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Plan + "/AdditinalLinkedDocument")]
        [ResponseType(typeof(List<FrontDocument>))]
        public async Task<IHttpActionResult> AdditinalLinkedDocumentSendLists([FromUri]AdditinalLinkedDocumentSendList model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
                   var items = docProc.GetAdditinalLinkedDocumentSendLists(context, model);
                   var res = new JsonResult(items, this);
                   return res;
               });
        }

        /// <summary>
        /// Принудительно запускает пункт плана на исполнение
        /// </summary>
        /// <param name="model">ИД пункта плана</param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Plan + "/LaunchItem")]
        public async Task<IHttpActionResult> LaunchItem([FromBody]Item model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var aplan = DmsResolver.Current.Get<IAutoPlanService>();
                   aplan.ManualRunAutoPlan(context, model.Id, null);
                   var res = new JsonResult(model.Id, this);
                   return res;
               });
        }

        /// <summary>
        /// Запускает автоматическую отработку плана
        /// </summary>
        /// <param name="model">ИД документа</param>
        /// <returns></returns>
        [Route(Features.Plan + "/LaunchPlan")]
        [HttpPut]
        public async Task<IHttpActionResult> LaunchPlan([FromBody]Item model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.LaunchPlan, model.Id);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Останавливет автоматическую отработку плана
        /// </summary>
        /// <param name="model">ИД документа</param>
        /// <returns></returns>
        [Route(Features.Plan + "/StopPlan")]
        [HttpPut]
        public async Task<IHttpActionResult> StopPlan([FromBody]Item model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.StopPlan, model.Id);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

    }
}
