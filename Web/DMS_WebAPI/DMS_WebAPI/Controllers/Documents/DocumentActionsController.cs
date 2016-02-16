using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix("api/v2/DocumentActions")]
    public class DocumentActionsController : ApiController
    {

        /// <summary>
        /// Получение списка доступных команд по документу
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns>Массив команд</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            var actions = docProc.GetDocumentActions(cxt, id);
            return new JsonResult(actions, this);
        }

        /// <summary>
        /// Изменение признака включения в Избранное
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route("ChangeFavourites")]
        [HttpPost]
        public IHttpActionResult ChangeFavourites(ChangeFavourites model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            docProc.ChangeFavouritesForDocument(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }
        /// <summary>
        /// Регистрация документа
        /// Возможности:
        /// 1. Получить регистрационный номер
        /// 2. Зарегистрировать документ
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route("RegisterDocument")]
        [HttpPost]
        public IHttpActionResult RegisterDocument(RegisterDocument model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            docProc.RegisterDocument(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }
        /// <summary>
        /// Добавление связи между документами
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route("AddDocumentLink")]
        [HttpPost]
        public IHttpActionResult AddDocumentLink(AddDocumentLink model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            docProc.AddDocumentLink(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Окончание/возобновление работы с документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ChangeWorkStatus")]
        [HttpPost]
        public IHttpActionResult ChangeWorkStatus(ChangeWorkStatus model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            docProc.ChangeDocumentWorkStatus(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Добавление примечания
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddNote")]
        [HttpPost]
        public IHttpActionResult AddNote(AddNote model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            docProc.AddDocumentComment(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Взятие на контроль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ControlOn")]
        [HttpPost]
        public IHttpActionResult ControlOn(ControlOn model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            docProc.ControlOn(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Изменить контроль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ControlChange")]
        [HttpPost]
        public IHttpActionResult ControlChange(ControlChange model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            docProc.ControlChange(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Снять с контроль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ControlOff")]
        [HttpPost]
        public IHttpActionResult ControlOff(ControlOff model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            docProc.ControlOff(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Копирование документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Добавленный документ</returns>
        [Route("CopyDocument")]
        [HttpPost]
        public IHttpActionResult CopyDocument(CopyDocument model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentOperationsService>();
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(docProc.CopyDocument(cxt, model));
        }
    }
}
