using System.ComponentModel.DataAnnotations;
using BL.Logic.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Web.Http.Description;
using System.Collections.Generic;
using BL.Model.SystemCore;
using BL.Model.DocumentCore.Filters;

namespace DMS_WebAPI.Controllers
{
    /// <summary>
    /// Шаблоны документов
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "TemplateDocuments")]
    public class TemplateDocumentsController : ApiController
    {
        /// <summary>
        /// Получение списка шаблонов документов
        /// </summary>
        /// <returns>Список шаблонов документов</returns>
        [ResponseType(typeof(List<FrontTemplateDocument>))]
        public IHttpActionResult Get([FromUri] FilterTemplateDocument filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDocs = tmpDocProc.GetTemplateDocuments(ctx, filter, paging);
            var res = new JsonResult(tmpDocs, this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Получение шаблона документа по ИД
        /// </summary>
        /// <param name="id">ИД шаблона документа</param>
        /// <returns>Шаблон документа</returns>
        [ResponseType(typeof(FrontTemplateDocument))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            var tmpDoc = tmpDocProc.GetTemplateDocument(ctx, id);

            var metaData = tmpDocProc.GetModifyMetaData(ctx, tmpDoc);

            return new JsonResult(tmpDoc, metaData, this);
        }

        /// <summary>
        /// Добавление шаблона документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyTemplateDocument model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.AddTemplateDocument,ctx,model);
            return Get((int)tmpTemplate);
        }

        /// <summary>
        /// Добавление шаблона документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("CopyTemplate/{id}")]
        [HttpPost]
        public IHttpActionResult Copy(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.CopyTemplateDocument, ctx, id);
            return Get((int)tmpTemplate);
        }

        /// <summary>
        /// Изменение шаблона документа
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put([Required]int id, [FromBody]ModifyTemplateDocument model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.ModifyTemplateDocument, ctx, model);
            return Get((int)tmpTemplate);
        }

       /// <summary>
       /// Удаление шаблона документа
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            tmpDocProc.ExecuteAction(EnumDocumentActions.DeleteTemplateDocument, ctx, id);

            var tmp = new FrontTemplateDocument();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}
