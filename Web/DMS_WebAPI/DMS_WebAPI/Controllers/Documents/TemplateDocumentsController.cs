using System.ComponentModel.DataAnnotations;
using BL.Logic.DocumentCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;

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


        public IHttpActionResult Post([FromBody]ModifyTemplateDocument model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.AddOrUpdateTemplate(cxt,model);
            return Get(tmpTemplate);
        }
       
        public IHttpActionResult Put([Required]int id, [FromBody]ModifyTemplateDocument model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpTemplate = tmpDocProc.AddOrUpdateTemplate(cxt, model);
            return Get(tmpTemplate);
        }

       
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDocProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            tmpDocProc.DeleteTemplate(cxt, id);

            FrontTemplateDocument tmp = new FrontTemplateDocument();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}
