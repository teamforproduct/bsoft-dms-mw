using System.Collections.Generic;
using System.Linq;
using BL.Logic.DocumentCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.DocumentCore.Filters;
using BL.Model.SystemCore;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DocumentPapers")]
    public class DocumentPapersController : ApiController
    {
        /// <summary>
        /// Получение Paper use V3
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Paper</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetDocumentPaper(ctx, id),this);
        }

        /// <summary>
        /// Получение списка Papers для документов use V3
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns>Список Papers</returns>
        public IHttpActionResult Get([FromUri]FilterDocumentPaper filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetDocumentPapers(ctx, filter, paging), this);
        }

        /// <summary>
        /// Добавление записи Papers use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Post([FromBody]AddDocumentPapers model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var newIds = (List<int>)docProc.ExecuteAction(EnumDocumentActions.AddDocumentPaper, ctx, new ModifyDocumentPapers(model));
            return Get(new FilterDocumentPaper {Id = newIds }, null);
        }

        /// <summary>
        /// Изменение записи Papers use V3
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentPapers model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentPaper, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи Papers use V3
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentPaper, ctx, id);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Отметить нахождение бумажного носителя у себя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("MarkOwnerDocumentPaper")]
        [HttpPost]
        public IHttpActionResult MarkOwnerDocumentPaper(MarkOwnerDocumentPaper model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.MarkOwnerDocumentPaper, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Отметить порчу бумажного носителя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("MarkСorruptionDocumentPaper")]
        [HttpPost]
        public IHttpActionResult MarkСorruptionDocumentPaper(PaperEvent model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.MarkСorruptionDocumentPaper, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Планировать движение бумажного носителя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("PlanDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult PlanDocumentPaperEvent(List<PlanMovementPaper> model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.PlanDocumentPaperEvent, ctx, model);
            return Get(new FilterDocumentPaper { Id = model.Select(x => x.Id).ToList() }, null);
        }

        /// <summary>
        /// Отменить планирование движения бумажного носителя
        /// </summary>
        /// <param name="model">перечень бумажных носителей</param>
        /// <returns></returns>
        [Route("CancelPlanDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult CancelPlanDocumentPaperEvent(PaperList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.CancelPlanDocumentPaperEvent, ctx, model);
            return Get( new FilterDocumentPaper {Id = model.PaperId, PaperListId = model.PaperListId}, null);
        }

        /// <summary>
        /// Отметить передачу бумажного носителя
        /// </summary>
        /// <param name="model">перечень бумажных носителей</param>
        /// <returns></returns>
        [Route("SendDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult SendDocumentPaperEvent(PaperList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.SendDocumentPaperEvent, ctx, model);
            return Get(new FilterDocumentPaper { Id = model.PaperId, PaperListId = model.PaperListId }, null);
        }

        /// <summary>
        /// Отменить передачу бумажного носителя
        /// </summary>
        /// <param name="model">перечень бумажных носителей</param>
        /// <returns></returns>
        [Route("CancelSendDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult CancelSendDocumentPaperEvent(PaperList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.CancelSendDocumentPaperEvent, ctx, model);
            return Get(new FilterDocumentPaper { Id = model.PaperId, PaperListId = model.PaperListId }, null);
        }

        /// <summary>
        /// Отметить прием бумажного носителя
        /// </summary>
        /// <param name="model">перечень бумажных носителей</param>
        /// <returns></returns>
        [Route("RecieveDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult RecieveDocumentPaperEvent(PaperList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.RecieveDocumentPaperEvent, ctx, model);
            return Get(new FilterDocumentPaper { Id = model.PaperId, PaperListId = model.PaperListId }, null);
        }

        /// <summary>
        /// Получение списка доступных команд по документу use V3
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns>Массив команд</returns>
        [Route("Actions/{id}")]
        [HttpGet]
        public IHttpActionResult Actions(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var cmdService = DmsResolver.Current.Get<ICommandService>();
            var actions = cmdService.GetDocumentPaperActions(ctx, id);

            return new JsonResult(actions, this);
        }

    }
}
