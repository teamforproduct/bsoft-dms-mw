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
    public class DocumentTasksController : ApiController
    {
        /// <summary>
        /// Получение Task
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Task</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentTaskService>();
            return new JsonResult(docProc.GetDocumentTask(ctx, id),this);
        }

        /// <summary>
        /// Получение списка Tasks для документов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список Tasks</returns>
        public IHttpActionResult Get([FromUri]FilterDocumentTask filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentTaskService>();
            return new JsonResult(docProc.GetTasks(ctx, filter, paging), this);
        }

        /// <summary>
        /// Добавление записи Task
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentTasks model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var newId = (int)docProc.ExecuteAction(EnumDocumentActions.AddDocumentTask, ctx, model);
            return Get(newId);
        }

        /// <summary>
        /// Изменение записи Task
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDocumentTasks model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentTask, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи Task
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentTask, ctx, id);
            return new JsonResult(null, this);
        }
    }
}
