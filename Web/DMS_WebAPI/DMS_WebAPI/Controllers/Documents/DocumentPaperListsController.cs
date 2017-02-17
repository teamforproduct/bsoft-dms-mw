using System.Collections.Generic;
using BL.Logic.DocumentCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.DocumentCore.Filters;
using BL.Model.SystemCore;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentPaperListsController : ApiController
    {
        /// <summary>
        /// Получение PaperList use V3
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PaperList</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetDocumentPaperList(ctx, id),this);
        }

        /// <summary>
        /// Получение списка PaperLists для документов use V3
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список PaperLists</returns>
        public IHttpActionResult Get([FromUri]FilterDocumentPaperList filter, UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            return new JsonResult(docProc.GetDocumentPaperLists(ctx, filter, paging), this);
        }

        /// <summary>
        /// Добавление записей PaperLists use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Post([FromBody]AddDocumentPaperList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var newIds = (List<int>)docProc.ExecuteAction(EnumDocumentActions.AddDocumentPaperList, ctx, model);
            return Get(new FilterDocumentPaperList { PaperListId = newIds }, null);
        }

        /// <summary>
        /// Изменение записи PaperLists use V3
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentPaperList model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentPaperList, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи PaperLists use V3
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentPaperList, ctx, id);
            return new JsonResult(null, this);
        }
    }
}
