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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var cmdService = DmsResolver.Current.Get<ICommandService>();
            var actions = cmdService.GetDocumentActions(ctx, id);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteFavourite, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddFavourite, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.RegisterDocument, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddDocumentLink, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentLink, ctx, id);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.StartWork, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.FinishWork, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.SendMessage, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddNote, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ControlOn, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.ControlChange, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.SendForExecutionChange, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.SendForControlChange, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.SendForResponsibleExecutionChange, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.ControlTargetChange, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.ControlOff, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.CopyDocument, ctx, model);
            
            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ChangeExecutor, ctx, model);

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ChangePosition, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            //TODO change
            docProc.ExecuteAction(EnumDocumentActions.LaunchPlan, ctx, id);
            
            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(id);
        }

        /// <summary>
        /// отметить для пользователя ивенты документа как прочтенные 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("MarkDocumentEventAsRead")]
        [HttpPost]
        public IHttpActionResult MarkDocumentEventAsRead(MarkDocumentEventAsRead model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.MarkDocumentEventAsRead, ctx, model);
            
            return new JsonResult(null, this);
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            //TODO change
            docProc.ExecuteAction(EnumDocumentActions.StopPlan, ctx, id);
            
            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.MarkExecution, ctx, model);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectResult, ctx, model);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AcceptResult, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectSigning, ctx, model);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectVisaing, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectАgreement, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectАpproval, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.WithdrawSigning, ctx, model);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.WithdrawVisaing, ctx, model);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.WithdrawАgreement, ctx, model);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.WithdrawАpproval, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AffixSigning, ctx, model);
            
            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }

        /// <summary>
        /// Самоподписание
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SelfAffixSigning")]
        [HttpPost]
        public IHttpActionResult SelfAffixSigning(SelfAffixSigning model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.SelfAffixSigning, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AffixVisaing, ctx, model);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AffixАgreement, ctx, model);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.AffixАpproval, ctx, model);

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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var res = docProc.ExecuteAction(EnumDocumentActions.ReportRegistrationCardDocument, ctx, id);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var res = docProc.ExecuteAction(EnumDocumentActions.ReportRegisterTransmissionDocuments, ctx, id);
            
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
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var ap = DmsResolver.Current.Get<IAutoPlanService>();
            var res = ap.ManualRunAutoPlan(ctx);
            
            return new JsonResult(res, this);
        }

    }
}
