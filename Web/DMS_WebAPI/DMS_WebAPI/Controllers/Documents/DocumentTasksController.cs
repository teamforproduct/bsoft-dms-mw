using BL.Logic.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.DocumentCore.Filters;

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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentTaskService>();
            return new JsonResult(docProc.GetDocumentTask(cxt, id),this);
        }

        /// <summary>
        /// Получение списка Tasks для документов
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список Tasks</returns>
        public IHttpActionResult Get([FromUri]FilterDocumentTask filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentTaskService>();
            return new JsonResult(docProc.GetTasks(cxt, filter), this);
        }

        /// <summary>
        /// Добавление записи Task
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Измененная запись</returns>
        public IHttpActionResult Post([FromBody]ModifyDocumentTasks model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddDocumentTask, cxt, model);
            return Get(model.Id);
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentTask, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи Task
        /// </summary>
        /// <param name="id">ИД записи</param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            int docId = (int)docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentTask, cxt, id);
            return Get(docId);
        }
    }
}
