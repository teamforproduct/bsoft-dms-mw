using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Enums;

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
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var cmdService = DmsResolver.Current.Get<ICommandService>();
            timeDB.Start();
            var actions = cmdService.GetDocumentActions(ctx, id);
            timeDB.Stop();
            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController Get List", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService GetDocumentActionsPrepare", timeDB.Elapsed);

            return new JsonResult(actions, this);
        }

        /// <summary>
        /// Удаление в Избранного
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route("DeleteFavourite")]
        [HttpPost]
        public IHttpActionResult DeleteFavourite(ChangeFavourites model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.DeleteFavourite, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController DeleteFavourite", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService DeleteFavourite", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(model.DocumentId);
        }
        /// <summary>
        /// Добавление в Избранное
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route("AddFavourite")]
        [HttpPost]
        public IHttpActionResult AddFavourite(ChangeFavourites model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.AddFavourite, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController AddFavourite", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService AddFavourite", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
        [Route("GetNextRegisterDocumentNumber")]
        [HttpPost]
        public IHttpActionResult GetNextRegisterDocumentNumber(RegisterDocumentBase model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var nextNumb = docProc.GetNextRegisterDocumentNumber(ctx, model);
            return new JsonResult(nextNumb, this);
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
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.RegisterDocument, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController RegisterDocument", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService RegisterDocument", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
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
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.AddDocumentLink, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController AddDocumentLink", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService AddDocumentLink", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Удаление связи между документами
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns>Обновленный документ</returns>
        [Route("DeleteDocumentLink/{id}")]
        [HttpPost]
        public IHttpActionResult DeleteDocumentLink(int id)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentLink, ctx, id);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController DeleteDocumentLink", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService DeleteDocumentLink", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(id);
        }

        /// <summary>
        /// Возобновление работы с документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("StartWork")]
        [HttpPost]
        public IHttpActionResult StartWork(ChangeWorkStatus model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.StartWork, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController StartWork", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService StartWork", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Окончание работы с документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("FinishWork")]
        [HttpPost]
        public IHttpActionResult FinishWork(ChangeWorkStatus model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.FinishWork, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController FinishWork", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService FinishWork", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Отправка сообщения группе
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SendMessage")]
        [HttpPost]
        public IHttpActionResult SendMessage(SendMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.SendMessage, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController SendMessage", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService SendMessage", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
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
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.AddNote, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController AddNote", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService AddDocumentComment", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
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
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.ControlOn, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController ControlOn", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService ControlOn", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
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
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.ControlChange,ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController ControlChange", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService ControlChange", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(documentId);
        }


        /// <summary>
        /// Изменить параметры направлен для исполнения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SendForExecutionChange")]
        [HttpPost]
        public IHttpActionResult SendForExecutionChange(ControlChange model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.SendForExecutionChange, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController SendForExecutionChange", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService SendForExecutionChange", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(documentId);
        }

        /// <summary>
        /// Изменить параметры направлен для исполнения (на контроль)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SendForControlChange")]
        [HttpPost]
        public IHttpActionResult SendForControlChange(ControlChange model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.SendForControlChange, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController SendForControlChange", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService SendForControlChange", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(documentId);
        }

        /// <summary>
        /// Изменить параметры направлен для исполнения (отв. исполнитель)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SendForResponsibleExecutionChange")]
        [HttpPost]
        public IHttpActionResult SendForResponsibleExecutionChange(ControlChange model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.SendForResponsibleExecutionChange, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController SendForResponsibleExecutionChange", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService SendForResponsibleExecutionChange", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(documentId);
        }

        /// <summary>
        /// Изменить контроль для исполнителя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("ControlTargetChange")]
        [HttpPost]
        public IHttpActionResult ControlTargetChange(ControlTargetChange model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.ControlTargetChange, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController ControlTargetChange", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService ControlTargetChange", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(documentId);
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
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.ControlOff, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController ControlOff", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService ControlOff", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(documentId);
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
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.CopyDocument, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController CopyDocument", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService CopyDocument", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Изменение исполнителя по документу
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route("ChangeExecutor")]
        [HttpPost]
        public IHttpActionResult ChangeExecutor(ChangeExecutor model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.ChangeExecutor, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController ChangeExecutor", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService ChangeExecutor", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Замена должности по документу
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route("ChangePosition")]
        [HttpPost]
        public IHttpActionResult ChangePosition(ChangePosition model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.ChangePosition, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController ChangePosition", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService ChangePosition", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Запустить план
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns></returns>
        [Route("LaunchPlan/{id}")]
        [HttpPost]
        public IHttpActionResult LaunchPlan(int id)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            //TODO change
            docProc.ExecuteAction(EnumDocumentActions.LaunchPlan, ctx, id);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController LaunchPlan", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService LaunchPlan", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(id);
        }

        /// <summary>
        /// отметить для пользователя ивенты документа как прочтенные 
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns></returns>
        [Route("MarkDocumentEventAsRead/{id}")]
        [HttpPost]
        public IHttpActionResult MarkDocumentEventAsRead(int id)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.MarkDocumentEventAsRead, ctx, id);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController MarkDocumentEventAsRead", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService MarkDocumentEventAsRead", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(id);
        }

        /// <summary>
        /// Остановить план
        /// </summary>
        /// <param name="id">ИД документа</param>>
        /// <returns>Обновленный документ</returns>
        [Route("StopPlan/{id}")]
        [HttpPost]
        public IHttpActionResult StopPlan(int id)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            //TODO change
            docProc.ExecuteAction(EnumDocumentActions.StopPlan, ctx, id);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController StopPlan", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService StopPlan", timeDB.Elapsed);

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(id);
        }

        /// <summary>
        /// Отметить выполнение поручения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("MarkExecution")]
        [HttpPost]
        public IHttpActionResult MarkExecution(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.MarkExecution, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController MarkExecution", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService MarkExecution", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }

        /// <summary>
        /// Отклонить прием результата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RejectResult")]
        [HttpPost]
        public IHttpActionResult RejectResult(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectResult, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController RejectResult", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService RejectResult", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }

        /// <summary>
        /// Отклонить прием результата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AcceptResult")]
        [HttpPost]
        public IHttpActionResult AcceptResult(ControlOff model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AcceptResult, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController AcceptResult", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService AcceptResult", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }

        /// <summary>
        /// Отклонить подписание
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RejectSigning")]
        [HttpPost]
        public IHttpActionResult RejectSigning(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectSigning, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController RejectSigning", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService RejectSigning", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Отклонить визирование
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RejectVisaing")]
        [HttpPost]
        public IHttpActionResult RejectVisaing(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectVisaing, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController RejectVisaing", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService RejectVisaing", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Отклонить согласование
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RejectАgreement")]
        [HttpPost]
        public IHttpActionResult RejectАgreement(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectАgreement, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController RejectАgreement", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService RejectАgreement", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Отклонить утверждение
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RejectАpproval")]
        [HttpPost]
        public IHttpActionResult RejectАpproval(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectАpproval, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController RejectАpproval", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService RejectАpproval", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }

        /// <summary>
        /// Отозвать с подписания
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("WithdrawSigning")]
        [HttpPost]
        public IHttpActionResult WithdrawSigning(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.WithdrawSigning, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController WithdrawSigning", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService WithdrawSigning", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Отозвать с визирования
        ///  </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("WithdrawVisaing")]
        [HttpPost]
        public IHttpActionResult WithdrawVisaing(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.WithdrawVisaing, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController WithdrawVisaing", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService WithdrawVisaing", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Отозвать с согласования
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("WithdrawАgreement")]
        [HttpPost]
        public IHttpActionResult WithdrawАgreement(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.WithdrawАgreement, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController WithdrawАgreement", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService WithdrawАgreement", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Отозвать с утверждения
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("WithdrawАpproval")]
        [HttpPost]
        public IHttpActionResult WithdrawАpproval(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.WithdrawАpproval, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController WithdrawАpproval", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService WithdrawАpproval", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }

        /// <summary>
        /// Подписать
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AffixSigning")]
        [HttpPost]
        public IHttpActionResult AffixSigning(AffixSigning model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AffixSigning, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController AffixSigning", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService AffixSigning", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Завизировать
        ///  </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AffixVisaing")]
        [HttpPost]
        public IHttpActionResult AffixVisaing(AffixSigning model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AffixVisaing, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController AffixVisaing", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService AffixVisaing", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Согласовать
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AffixАgreement")]
        [HttpPost]
        public IHttpActionResult AffixАgreement(AffixSigning model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AffixАgreement, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController AffixАgreement", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService AffixАgreement", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }
        /// <summary>
        /// Утвердить
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AffixАpproval")]
        [HttpPost]
        public IHttpActionResult AffixАpproval(AffixSigning model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AffixАpproval, ctx, model);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController AffixАpproval", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService AffixАpproval", timeDB.Elapsed);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }

        /// <summary>
        /// Получить отчет
        /// </summary>
        /// <param name="id">ИД документа</param>>
        /// <returns></returns>
        [Route("ReportRegistrationCardDocument/{id}")]
        [HttpPost]
        public IHttpActionResult ReportRegistrationCardDocument(int id)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var res = docProc.ExecuteAction(EnumDocumentActions.ReportRegistrationCardDocument, ctx, id);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController ReportRegistrationCardDocument", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService ReportRegistrationCardDocument", timeDB.Elapsed);

            return new JsonResult(res, this);
        }

        /// <summary>
        /// Получить отчет Реестр передачи документов
        /// </summary>
        /// <param name="id">ИД PaperList</param>>
        /// <returns></returns>
        [Route("ReportRegisterTransmissionDocuments/{id}")]
        [HttpPost]
        public IHttpActionResult ReportRegisterTransmissionDocuments(int id)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var res = docProc.ExecuteAction(EnumDocumentActions.ReportRegisterTransmissionDocuments, ctx, id);
            timeDB.Stop();

            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController ReportRegisterTransmissionDocuments", timeM.Elapsed);
            BL.CrossCutting.Helpers.Logger.SaveToFile("DB: IDocumentOperationsService ReportRegisterTransmissionDocuments", timeDB.Elapsed);

            return new JsonResult(res, this);
        }

        /// <summary>
        /// Запустить автоматические планы вручную
        /// </summary>
        /// <param name="id">ИД PaperList</param>>
        /// <returns></returns>
        [Route("ManualStartAutoPlan")]
        [HttpPost]
        public IHttpActionResult ManualStartAutoPlan()
        {
            var timeM = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var ap = DmsResolver.Current.Get<IAutoPlanService>();
            var res = ap.ManualRunAutoPlan(ctx);
            timeM.Stop();
            BL.CrossCutting.Helpers.Logger.SaveToFile("M: DocumentActionsController ManualStartAutoPlan", timeM.Elapsed);

            return new JsonResult(res, this);
        }

    }
}
