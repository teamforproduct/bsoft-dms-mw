using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
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
    /// Документы. Бумажные носители.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentPaperController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocumentPaper(context, Id);
            var res = new JsonResult(item, this);
            return res;
        }

        /// <summary>
        /// Возвращает список бумажных носителей
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="paging">Пейджинг</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Papers + "/Main")]
        [ResponseType(typeof(List<FrontDocumentPaper>))]
        public async Task<IHttpActionResult> Get([FromUri]FilterDocumentPaper filter, [FromUri]UIPaging paging)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentService>();
                   var items = docProc.GetDocumentPapers(context, filter, paging);
                   var res = new JsonResult(items, this);
                   return res;
               });
        }

        /// <summary>
        /// Возвращает бумажный носитель по ИД
        /// </summary>
        /// <param name="Id">ИД БН</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Papers + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentPaper))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет бумажный носитель
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Papers)]
        public async Task<IHttpActionResult> Post([FromBody]AddDocumentPaper model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.Execute(context, EnumDocumentActions.AddDocumentPaper, model, model.CurrentPositionId);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Измененяет бумажный носитель
        /// </summary>
        /// <param name="model">Модель для обновления</param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Papers)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyDocumentPaper model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.Execute(context, EnumDocumentActions.ModifyDocumentPaper, model);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Удаляет бумажный носитель
        /// </summary>
        /// <param name="Id">ИД БН</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Papers + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.DeleteDocumentPaper, Id);
                   var tmpItem = new FrontDeleteModel(Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Возвращает меню по ИД документа для работы с БН 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Papers + "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public async Task<IHttpActionResult> Actions([FromUri]int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<ICommandService>();
                   var items = docProc.GetDocumentPaperActions(context, Id);
                   var res = new JsonResult(items, this);
                   return res;
               });
        }

        /// <summary>
        /// Отмечает нахождение бумажного носителя у себя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Papers + "/MarkOwner")]
        [HttpPut]
        public async Task<IHttpActionResult> MarkOwnerDocumentPaper([FromBody]MarkOwnerDocumentPaper model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.MarkOwnerDocumentPaper, model, model.CurrentPositionId);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Отмечает порчу бумажного носителя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Papers + "/MarkCorruption")]
        [HttpPut]
        public async Task<IHttpActionResult> MarkСorruptionDocumentPaper([FromBody]PaperEvent model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.MarkСorruptionDocumentPaper, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Планирует движение бумажного носителя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Papers + "/Plan")]
        [HttpPut]
        public async Task<IHttpActionResult> PlanDocumentPaperEvent([FromBody]List<PlanMovementPaper> model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.PlanDocumentPaperEvent, model);
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Отменяет планирование движения бумажного носителя
        /// </summary>
        /// <param name="model">массив ИД БН</param>
        /// <returns></returns>
        [Route(Features.Papers + "/CancelPlan")]
        [HttpPut]
        public async Task<IHttpActionResult> CancelPlanDocumentPaperEvent([FromBody]List<int> model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.CancelPlanDocumentPaperEvent, new PaperList { PaperId = model });
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Отмечает передачу бумажного носителя
        /// </summary>
        /// <param name="model">массив ИД БН</param>
        /// <returns></returns>
        [Route(Features.Papers + "/Send")]
        [HttpPut]
        public async Task<IHttpActionResult> SendDocumentPaperEvent([FromBody]List<int> model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.SendDocumentPaperEvent, new PaperList { PaperId = model });
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        ///  Отменяет передачу бумажного носителя
        /// </summary>
        /// <param name="model">массив ИД БН</param>
        /// <returns></returns>
        [Route(Features.Papers + "/CancelSend")]
        [HttpPut]
        public async Task<IHttpActionResult> CancelSendDocumentPaperEvent([FromBody]List<int> model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.CancelSendDocumentPaperEvent, new PaperList { PaperId = model });
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Отмечает прием бумажного носителя
        /// </summary>
        /// <param name="model">массив ИД БН</param>
        /// <returns></returns>
        [Route(Features.Papers + "/Recieve")]
        [HttpPut]
        public async Task<IHttpActionResult> RecieveDocumentPaperEvent([FromBody]List<int> model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.RecieveDocumentPaperEvent, new PaperList { PaperId = model });
                   var res = new JsonResult(null, this);
                   return res;
               });
        }


    }
}
