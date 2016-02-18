using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.DocumentCore.IncomingModel;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentTagsController : ApiController
    {
        /// <summary>
        /// Получение тег документа
        /// Зависит от выставленных позиций
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Тег документа</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tagProc = DmsResolver.Current.Get<IDocumentTagService>();
            return new JsonResult(tagProc.GetTag(cxt, id),this);
        }

        /// <summary>
        /// Получение тегов документа
        /// Зависит от выставленных позиций
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns>Теги документа</returns>
        public IHttpActionResult GetByDocument(int documentId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tagProc = DmsResolver.Current.Get<IDocumentTagService>();
            return new JsonResult(tagProc.GetTags(cxt, documentId), this);
        }

        /*
        /// <summary>
        /// Добавление записи плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись плана работы над документом</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            return Get(docProc.AddSendList(cxt, model));
        }

        /// <summary>
        /// Добавление плана работы над документом по стандартному списку
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененные записи плана работы над документом</returns>
        public IHttpActionResult Put([FromBody]ModifyDocumentSendListByStandartSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            docProc.AddSendListByStandartSendLists(cxt, model);
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
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            docProc.UpdateSendList(cxt, model);
            return Get(id);
        }

        /// <summary>
        /// Удаление записи плана работы над документом
        /// </summary>
        /// <param name="id">ИД записи плана работы над документом</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            docProc.DeleteSendList(cxt, id);
            return new JsonResult(null, this);
        }
        */
    }
}
