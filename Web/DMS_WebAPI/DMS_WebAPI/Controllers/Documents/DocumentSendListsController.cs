using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentSendListsController : ApiController
    {
        /// <summary>
        /// Добавление записи плана работы над документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный документ</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var restrictedSendListId = docProc.AddSendList(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Добавление плана работы над документом по стандартному списку
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененный документ</returns>
        public IHttpActionResult Put([FromBody]ModifyDocumentSendListByStandartSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.AddSendListByStandartSendLists(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
        }

        /// <summary>
        /// Изменение записи плана работы над документом
        /// </summary>
        /// <param name="id">ИД записи плана работы над документом</param>
        /// <param name="model"></param>
        /// <returns>Измененный документ</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentSendList model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.UpdateSendList(cxt, model);
            var ctrl = new DocumentsController();
            ctrl.ControllerContext = ControllerContext;
            return ctrl.Get(model.DocumentId);
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
            docProc.DeleteSendList(cxt, id);
            return new JsonResult(null, this);
        }
    }
}
