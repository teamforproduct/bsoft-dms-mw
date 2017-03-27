using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.DocumentTemplates
{
    /// <summary>
    /// Шаблоны документов. Задачи.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Templates)]
    public class TemplateTasksController : ApiController
    {
        /// <summary>
        /// Возвращает список задач
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Tasks)]
        [ResponseType(typeof(List<FrontTemplateDocumentTask>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterTemplateDocumentTask filter)
        {
            if (filter == null) filter = new FilterTemplateDocumentTask();
            filter.TemplateId =  Id ;

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItems = tmpService.GetTemplateDocumentTasks(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }


        /// <summary>
        /// Возвращает задачу по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Tasks + "/{Id:int}")]
        [ResponseType(typeof(FrontTemplateDocumentTask))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = tmpService.GetTemplateDocumentTask(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Добавляет задачу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Tasks)]
        public IHttpActionResult Post([FromBody]AddTemplateDocumentTask model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.AddTemplateDocumentTask, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует задачу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Tasks)]
        public IHttpActionResult Put([FromBody]ModifyTemplateDocumentTask model)
        {
            Action.Execute(EnumDocumentActions.ModifyTemplateDocumentTask, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет задачу
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Tasks + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDocumentActions.DeleteTemplateDocumentTask, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
