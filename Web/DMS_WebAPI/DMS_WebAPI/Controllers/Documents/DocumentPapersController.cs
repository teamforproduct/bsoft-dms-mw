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

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix("api/v2/DocumentPapers")]
    public class DocumentPapersController : ApiController
    {
        /// <summary>
        /// Получение Paper
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Paper</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetDocumentPaper(cxt, id),this);
        }

        /// <summary>
        /// Получение списка Papers для документов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список Papers</returns>
        public IHttpActionResult Get([FromUri]FilterDocumentPaper filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetDocumentPapers(cxt, filter), this);
        }

        /// <summary>
        /// Добавление записи Papers
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentPapers model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddDocumentPaper, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Изменение записи Papers
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentPapers model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentPaper, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи Papers
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentPaper, cxt, id);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Отметить нахождение бумажного носителя у себя
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <returns></returns>
        [Route("MarkOwnerDocumentPaper")]
        [HttpPost]
        public IHttpActionResult MarkOwnerDocumentPaper(MarkOwnerDocumentPaper model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.MarkOwnerDocumentPaper, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Отметить порчу бумажного носителя
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <returns></returns>
        [Route("MarkСorruptionDocumentPaper")]
        [HttpPost]
        public IHttpActionResult MarkСorruptionDocumentPaper(PaperEvent model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.MarkСorruptionDocumentPaper, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Планировать движение бумажного носителя
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <returns></returns>
        [Route("PlanDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult PlanDocumentPaperEvent(List<PlanMovementPaper> model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.PlanDocumentPaperEvent, cxt, model);
            return Get(new FilterDocumentPaper { Id = model.Select(x => x.Id).ToList() });
        }

        /// <summary>
        /// Отменить планирование движения бумажного носителя
        /// </summary>
        /// <param name="id">ИД бумажного носителя</param>
        /// <returns></returns>
        [Route("CancelPlanDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult CancelPlanDocumentPaperEvent(List<int> id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.CancelPlanDocumentPaperEvent, cxt, id);
            return Get( new FilterDocumentPaper {Id = id});
        }

        /// <summary>
        /// Отметить передачу бумажного носителя
        /// </summary>
        /// <param name="id">ИД бумажного носителя</param>
        /// <returns></returns>
        [Route("SendDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult SendDocumentPaperEvent(List<int> id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.SendDocumentPaperEvent, cxt, id);
            return Get(new FilterDocumentPaper { Id = id });
        }

        /// <summary>
        /// Отменить передачу бумажного носителя
        /// </summary>
        /// <param name="id">ИД бумажного носителя</param>
        /// <returns></returns>
        [Route("CancelSendDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult CancelSendDocumentPaperEvent(List<int> id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.CancelSendDocumentPaperEvent, cxt, id);
            return Get(new FilterDocumentPaper { Id = id });
        }

        /// <summary>
        /// Отметить прием бумажного носителя
        /// </summary>
        /// <param name="id">ИД бумажного носителя</param>
        /// <returns></returns>
        [Route("RecieveDocumentPaperEvent")]
        [HttpPost]
        public IHttpActionResult RecieveDocumentPaperEvent(List<int> id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.RecieveDocumentPaperEvent, cxt, id);
            return Get(new FilterDocumentPaper { Id = id });
        }

        /// <summary>
        /// Получение списка доступных команд по документу
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns>Массив команд</returns>
        [Route("Actions/{id}")]
        [HttpGet]
        public IHttpActionResult Actions(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var cmdService = DmsResolver.Current.Get<ICommandService>();
            var actions = cmdService.GetDocumentPaperActions(cxt, id);

            return new JsonResult(actions, this);
        }

    }
}
