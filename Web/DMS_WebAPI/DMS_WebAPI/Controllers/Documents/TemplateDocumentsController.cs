using System.ComponentModel.DataAnnotations;
using BL.Logic.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers
{
    /// <summary>
    /// Шаблоны документов
    /// </summary>
    [Authorize]
    public class TemplateDocumentsController : ApiController
    {
        /// <summary>
        /// Получение списка шаблонов документов
        /// </summary>
        /// <returns>Список шаблонов документов</returns>
        public IHttpActionResult Get()
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpDocs = tmpDocProc.GetTemplateDocuments(cxt);
            return new JsonResult(tmpDocs, this);
        }

        /// <summary>
        /// Получение шаблона документа по ИД
        /// </summary>
        /// <param name="id">ИД шаблона документа</param>
        /// <returns>Шаблон документа</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            var tmpDoc = tmpDocProc.GetTemplateDocument(cxt, id);

            var metaData = tmpDocProc.GetModifyMetaData(cxt, tmpDoc);

            return new JsonResult(tmpDoc, metaData, this);
        }

        /// <summary>
        /// Добавление шаблона документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyTemplateDocument model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.AddTemplateDocument,cxt,model);
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.ExecuteAction(EnumDocumentActions.ModifyTemplateDocument, cxt, model);
            return Get((int)tmpTemplate);
        }

       /// <summary>
       /// Удаление шаблона документа
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            tmpDocProc.ExecuteAction(EnumDocumentActions.DeleteTemplateDocument, cxt, id);

            var tmp = new FrontTemplateDocument();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}
