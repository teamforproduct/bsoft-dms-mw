using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentSendListsController : ApiController
    {
        /// <summary>
        /// Получение записи плана работы над документом
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Запись плана работы над документом</returns>
        private IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            return new JsonResult(docProc.GetSendList(cxt, id),this);
        }

        /// <summary>
        /// Получение записей плана работы над документом
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns>Записи плана работы над документом</returns>
        private IHttpActionResult GetByDocument(int documentId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            return new JsonResult(docProc.GetSendLists(cxt, documentId), this);
        }

        /// <summary>
        /// Добавление записи плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись плана работы над документом</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAdditionAction(EnumDocumentAdditionActions.AddDocumentSendList, cxt, model);
            return GetByDocument(model.DocumentId);
        }

        /// <summary>
        /// Добавление плана работы над документом по стандартному списку
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененные записи плана работы над документом</returns>
        public IHttpActionResult Put([FromBody]ModifyDocumentSendListByStandartSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAdditionAction(EnumDocumentAdditionActions.AddByStandartSendListDocumentSendList, cxt, model);
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAdditionAction(EnumDocumentAdditionActions.ModifyDocumentSendList, cxt, model);
            return GetByDocument(model.DocumentId);
        }

        /// <summary>
        /// Удаление записи плана работы над документом
        /// </summary>
        /// <param name="id">ИД записи плана работы над документом</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            int docId = (int)docProc.ExecuteAdditionAction(EnumDocumentAdditionActions.DeleteDocumentSendList, cxt, id);
            return GetByDocument(docId);
        }
    }
}
