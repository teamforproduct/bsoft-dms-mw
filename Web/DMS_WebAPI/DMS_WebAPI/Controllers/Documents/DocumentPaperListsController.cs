using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.DocumentCore.Filters;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentPaperListsController : ApiController
    {
        /// <summary>
        /// Получение PaperList
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PaperList</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetDocumentPaperList(cxt, id),this);
        }

        /// <summary>
        /// Получение списка PaperLists для документов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список PaperLists</returns>
        public IHttpActionResult Get([FromUri]FilterDocumentPaperList filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetDocumentPaperLists(cxt, filter), this);
        }

        /// <summary>
        /// Добавление записи PaperLists
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentPaperLists model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddDocumentPaperList, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Изменение записи PaperLists
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentPaperLists model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentPaperList, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи PaperLists
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            int docId = (int)docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentPaperList, cxt, id);
            return Get(docId);
        }
    }
}
