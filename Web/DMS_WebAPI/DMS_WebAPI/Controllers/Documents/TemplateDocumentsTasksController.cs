using System.ComponentModel.DataAnnotations;
using BL.Logic.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDocs = tmpDocProc.GetTemplateDocumentTasks(cxt,templateId,filter);
            return new JsonResult(tmpDocs, this);
        }

        /// <summary>
        /// Получение задачи шаблона документа по ИД
        /// </summary>
        /// <param name="id">ИД списка рассылки</param>
        /// <returns>Шаблон документа</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDoc = tmpDocProc.GetTemplateDocumentTask(cxt, id);
            return new JsonResult(tmpDoc, this);
        }

        /// <summary>
        /// Добавление задачи к шаблону документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyTemplateDocumentTasks model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.AddOrUpdateTemplateTask(cxt, model);
            return Get(tmpTemplate);
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.AddOrUpdateTemplateTask(cxt, model);
            return Get(tmpTemplate);
        }

        /// <summary>
        /// Удаление задачи из шаблона документа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            tmpDocProc.DeleteTemplateTask(cxt, id);

            var tmp = new FrontTemplateDocumentTasks() {Id = id};

            return new JsonResult(tmp, this);

        }
    }
}
