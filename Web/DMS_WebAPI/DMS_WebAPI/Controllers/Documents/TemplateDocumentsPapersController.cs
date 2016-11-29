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
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Documents
{
    /// <summary>
    /// Задачи шаблонов документов
    /// </summary>
    [Authorize]
    public class TemplateDocumentsPapersController : ApiController
    {
        /// <summary>
        /// Получение всех БН шаблона документов
        /// </summary>
        /// <returns>Список шаблонов документов</returns>
        public IHttpActionResult Get([FromUri]FilterTemplateDocumentPaper filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDocs = tmpDocProc.GetTemplateDocumentPapers(ctx,filter);
            return new JsonResult(tmpDocs, this);
        }

        /// <summary>
        /// Получение БН шаблона документа по ИД
        /// </summary>
        /// <param name="id">ИД списка рассылки</param>
        /// <returns>Шаблон документа</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDoc = tmpDocProc.GetTemplateDocumentPaper(ctx, id);
            return new JsonResult(tmpDoc, this);
        }

        /// <summary>
        /// Добавление БН к шаблону документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyTemplateDocumentPaper model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var newIds = (List<int>)tmpDocProc.ExecuteAction(EnumDocumentActions.AddTemplateDocumentPaper,ctx,model);
            return Get(new FilterTemplateDocumentPaper { IDs = newIds });
        }

        /// <summary>
        /// Изменение БН шаблона документа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put([Required]int id, [FromBody]ModifyTemplateDocumentPaper model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            tmpDocProc.ExecuteAction(EnumDocumentActions.ModifyTemplateDocumentPaper, ctx, model);
            return Get(id);
        }

        /// <summary>
        /// Удаление БН из шаблона документа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            tmpDocProc.ExecuteAction(EnumDocumentActions.DeleteTemplateDocumentPaper, ctx, id);
            return new JsonResult(null, this);
        }
    }
}
