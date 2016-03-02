﻿using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Actions;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DocumentCore.InternalModel;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix("api/v2/DocumentActions")]
    public class DocumentActionsController : ApiController
    {
        private void SaveToFile(string method, string time)
        {
            try
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(System.Web.HttpContext.Current.Server.MapPath("~/SiteLog.txt"));
                try
                {
                    string line = $"{System.DateTime.Now.ToString("o")}\r\n method: {method}\r\n time:{time}";
                    sw.WriteLine(line);
                }
                catch
                {
                }
                finally
                {
                    sw.Close();
                }
            }
            catch
            {
            }
        }

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var actions = docProc.GetDocumentActions(cxt, id);
            timeDB.Stop();
            timeM.Stop();
            SaveToFile("M: DocumentActionsController Get List", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService GetDocumentActionsPrepare", timeDB.Elapsed.ToString("G"));

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

            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.DeleteFavourite, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController DeleteFavourite", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService DeleteFavourite", timeDB.Elapsed.ToString("G"));

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

            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.AddFavourite, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController AddFavourite", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService AddFavourite", timeDB.Elapsed.ToString("G"));

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
        [Route("RegisterDocument")]
        [HttpPost]
        public IHttpActionResult RegisterDocument(RegisterDocument model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();

            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.RegisterDocument, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController RegisterDocument", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService RegisterDocument", timeDB.Elapsed.ToString("G"));

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.AddDocumentLink, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController AddDocumentLink", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService AddDocumentLink", timeDB.Elapsed.ToString("G"));

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
            return ctrl.Get(model.DocumentId);
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.StartWork, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController StartWork", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService StartWork", timeDB.Elapsed.ToString("G"));

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.FinishWork, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController FinishWork", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService FinishWork", timeDB.Elapsed.ToString("G"));

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.SendMessage, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController SendMessage", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService SendMessage", timeDB.Elapsed.ToString("G"));

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.AddNote, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController AddNote", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService AddDocumentComment", timeDB.Elapsed.ToString("G"));

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.ControlOn, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController ControlOn", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService ControlOn", timeDB.Elapsed.ToString("G"));

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.ControlChange,cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController ControlChange", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService ControlChange", timeDB.Elapsed.ToString("G"));

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var documentId = (int)docProc.ExecuteAction(EnumDocumentActions.ControlOff, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController ControlOff", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService ControlOff", timeDB.Elapsed.ToString("G"));

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.CopyDocument, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController CopyDocument", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService CopyDocument", timeDB.Elapsed.ToString("G"));

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

            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            docProc.ExecuteAction(EnumDocumentActions.ChangeExecutor, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController ChangeExecutor", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService ChangeExecutor", timeDB.Elapsed.ToString("G"));

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
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

            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            //TODO change
            docProc.ExecuteAction(EnumDocumentActions.LaunchPlan, cxt, id);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController LaunchPlan", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService LaunchPlan", timeDB.Elapsed.ToString("G"));

            var ctrl = new DocumentsController {ControllerContext = ControllerContext};
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

            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            //TODO change
            docProc.ExecuteAction(EnumDocumentActions.StopPlan, cxt, id);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController StopPlan", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService StopPlan", timeDB.Elapsed.ToString("G"));

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.MarkExecution, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController MarkExecution", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService AddDocumentComment", timeDB.Elapsed.ToString("G"));

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }

        /// <summary>
        /// Отклонить прием результата
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("MarkExecution")]
        [HttpPost]
        public IHttpActionResult RejectResult(SendEventMessage model)
        {
            var timeM = new System.Diagnostics.Stopwatch();
            var timeDB = new System.Diagnostics.Stopwatch();
            timeM.Start();
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            timeDB.Start();
            var docId = (int)docProc.ExecuteAction(EnumDocumentActions.RejectResult, cxt, model);
            timeDB.Stop();

            timeM.Stop();
            SaveToFile("M: DocumentActionsController MarkExecution", timeM.Elapsed.ToString("G"));
            SaveToFile("DB: IDocumentOperationsService AddDocumentComment", timeDB.Elapsed.ToString("G"));

            var ctrl = new DocumentsController { ControllerContext = ControllerContext };
            return ctrl.Get(docId);
        }

    }
}
