using BL.Logic.DocumentCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Logic.SystemServices.AutoPlan;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DocumentSendLists")]
    public class DocumentSendListsController : ApiController
    {
        /// <summary>
        /// Получение записи плана работы над документом
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Запись плана работы над документом</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            return new JsonResult(docProc.GetSendList(ctx, id), this);
        }

        /// <summary>
        /// Получение записей плана работы над документом
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns>Записи плана работы над документом</returns>
        [Route("GetByDocument")]
        [HttpGet]
        public IHttpActionResult GetByDocument(int documentId)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            return new JsonResult(docProc.GetSendLists(ctx, documentId), this);
        }

        /// <summary>
        /// Добавление записи плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись плана работы над документом</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentSendList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var newId = docProc.ExecuteAction(EnumDocumentActions.AddDocumentSendList, ctx, model);
            if (newId == null)
                return new JsonResult(true, this);
            else
                return Get((int)newId);
        }

        /// <summary>
        /// Добавление плана работы над документом по стандартному списку
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененные записи плана работы над документом</returns>
        public IHttpActionResult Put([FromBody]ModifyDocumentSendListByStandartSendList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddByStandartSendListDocumentSendList, ctx, model);
            return GetByDocument(model.DocumentId);
        }

        /// <summary>
        /// Изменение записи плана работы над документом
        /// </summary>
        /// <param name="id">ИД записи плана работы над документом</param>
        /// <param name="model"></param>
        /// <returns>Измененная запись плана работы над документом</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentSendList model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentSendList, ctx, model);
            return Get(id);
        }

        /// <summary>
        /// Удаление записи плана работы над документом
        /// </summary>
        /// <param name="id">ИД записи плана работы над документом</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            int docId = (int)docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentSendList, ctx, id);
            return GetByDocument(docId);
        }

        /// <summary>
        /// Ручной запуск записи плана работы на исполнение
        /// </summary>
        /// <param name="id">ИД пункта плана</param>
        /// <returns></returns>
        [Route("LaunchItem/{id}")]
        [HttpPost]
        public IHttpActionResult LaunchItem(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var aplan = DmsResolver.Current.Get<IAutoPlanService>();
            aplan.ManualRunAutoPlan(ctx, id, null);
            return Get(id);
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
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var cmdService = DmsResolver.Current.Get<ICommandService>();
            var actions = cmdService.GetDocumentSendListActions(ctx, id);

            return new JsonResult(actions, this);
        }

        /// <summary>
        /// Получение списка доступных команд по документу
        /// </summary>
        /// <param name="model">модель</param>
        /// <returns>Массив команд</returns>
        [Route("AdditinalLinkedDocumentSendLists")]
        [HttpPost]
        public IHttpActionResult AdditinalLinkedDocumentSendLists([FromBody]AdditinalLinkedDocumentSendList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            var sendList = docProc.GetAdditinalLinkedDocumentSendLists(ctx, model);

            return new JsonResult(sendList, this);
        }

    }
}
