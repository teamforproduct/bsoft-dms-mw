using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
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
using System.Threading.Tasks;
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
    public class TemplateTasksController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var tmpService = DmsResolver.Current.Get<ITemplateService>();
            var tmpItem = tmpService.GetTemplateTask(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список задач
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Tasks)]
        [ResponseType(typeof(List<FrontTemplateTask>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterTemplateTask filter)
        {
            if (filter == null) filter = new FilterTemplateTask();
            filter.TemplateId = Id;

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<ITemplateService>();
                var tmpItems = tmpService.GetTemplateTasks(context, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }


        /// <summary>
        /// Возвращает задачу по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Tasks + "/{Id:int}")]
        [ResponseType(typeof(FrontTemplateTask))]
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
        public async Task<IHttpActionResult> Post([FromBody]AddTemplateTask model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.ExecuteDocumentAction(context, EnumActions.AddTemplateTask, model);
                   return GetById(context, tmpItem);
               });
        }

        /// <summary>
        /// Корректирует задачу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Tasks)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyTemplateTask model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.ExecuteDocumentAction(context, EnumActions.ModifyTemplateTask, model);
                   return GetById(context, model.Id);
               });
        }

        /// <summary>
        /// Удаляет задачу
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Tasks + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.ExecuteDocumentAction(context, EnumActions.DeleteTemplateTask, Id);
                   var tmpItem = new FrontDeleteModel(Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

    }
}
