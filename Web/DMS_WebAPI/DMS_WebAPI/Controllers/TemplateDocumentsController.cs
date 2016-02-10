using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers
{
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
            return new JsonResult(tmpDoc, this);
        }


        //public IHttpActionResult Post(BaseTemplateDocument model)
        //{
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var docProc = DmsResolver.Current.Get<ITemplateDocumentService>();
        //    docProc.AddOrUpdateTemplate(cxt, model);
        //    return Ok();
        //}
    }
}
