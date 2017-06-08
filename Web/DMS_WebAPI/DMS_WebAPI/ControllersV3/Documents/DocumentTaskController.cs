using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Задачи.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentTaskController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentTaskService>();
            var item = docProc.GetDocumentTask(context, Id);
            var res = new JsonResult(item, this);
            return res;
        }

        /// <summary>
        /// Возвращает список задач
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="paging">Пейджинг</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Tasks + "/Main")]
        [ResponseType(typeof(List<FrontDocumentTask>))]
        public async Task<IHttpActionResult> Get([FromUri]FilterDocumentTask filter, [FromUri]UIPaging paging)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentTaskService>();
                   var items = docProc.GetTasks(context, filter, paging);
                   var res = new JsonResult(items, this);
                   return res;
               });
        }

        /// <summary>
        /// Возвращает задачу по ИД
        /// </summary>
        /// <param name="Id">ИД задачи</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Tasks + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentTask))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет задачу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Tasks)]
        public async Task<IHttpActionResult> Post([FromBody]AddDocumentTask model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.ExecuteDocumentAction(context, EnumActions.AddDocumentTask, model, model.CurrentPositionId);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Измененяет задачу
        /// </summary>
        /// <param name="model">Модель для обновления</param>
        /// <returns>Обновленный пункт плана</returns>
        [HttpPut]
        [Route(Features.Tasks)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyDocumentTask model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.ExecuteDocumentAction(context, EnumActions.ModifyDocumentTask, model);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Удаляет задачу
        /// </summary>
        /// <param name="Id">ИД задачи</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Tasks + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.ExecuteDocumentAction(context, EnumActions.DeleteDocumentTask, Id);
                   var tmpItem = new FrontDeleteModel(Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

    }
}
