using System.ComponentModel.DataAnnotations;
using BL.Logic.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Documents
{
    /// <summary>
    /// Задачи шаблонов документов
    /// </summary>
    [Authorize]
    public class TemplateDocumentsTasksController : ApiController
    {
        /// <summary>
        /// Получение всех задач шаблона документов
        /// </summary>
        /// <returns>Список шаблонов документов</returns>
        public IHttpActionResult Get([Required]int templateId,[FromUri]FilterTemplateDocumentTask filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDocs = tmpDocProc.GetTemplateDocumentTasks(ctx,templateId,filter);
            return new JsonResult(tmpDocs, this);
        }

        /// <summary>
        /// Получение задачи шаблона документа по ИД
        /// </summary>
        /// <param name="id">ИД списка рассылки</param>
        /// <returns>Шаблон документа</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDoc = tmpDocProc.GetTemplateDocumentTask(ctx, id);
            return new JsonResult(tmpDoc, this);
        }

        /// <summary>
        /// Добавление задачи к шаблону документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyTemplateDocumentTasks model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.AddTemplateDocumentTask,ctx,model);
            return Get((int)tmpTemplate);
        }

        /// <summary>
        /// Изменение задачи шаблона документа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put([Required]int id, [FromBody]ModifyTemplateDocumentTasks model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.ModifyTemplateDocumentTask, ctx, model);
            return Get((int)tmpTemplate);
        }

        /// <summary>
        /// Удаление задачи из шаблона документа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            tmpDocProc.ExecuteAction(EnumDocumentActions.AddTemplateDocumentTask, ctx, id); 

            var tmp = new FrontTemplateDocumentTasks() {Id = id};

            return new JsonResult(tmp, this);

        }
    }
}
