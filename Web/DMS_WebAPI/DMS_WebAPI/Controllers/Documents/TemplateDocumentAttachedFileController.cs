using System.ComponentModel.DataAnnotations;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Web;

namespace DMS_WebAPI.Controllers.Documents
{
    /// <summary>
    /// Вложенные файлы в шаблонах документов
    /// </summary>
    [Authorize]
    public class TemplateDocumentAttachedFileController : ApiController
    {
        
        /// <summary>
        /// Получить список файлов шаблона
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Get([Required]int templateId,[FromUri]FilterTemplateAttachedFile model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            var res = docFileProc.GetTemplateAttachedFiles(cxt, model,templateId);

            return new JsonResult(res, this);
        }

        /// <summary>
        /// Получить вложенный в шаблон файл по ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            return new JsonResult(docFileProc.GetTemplateAttachedFile(cxt, id), this);
        }

       /// <summary>
       /// Добавить вложенный файл в шаблон
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        public IHttpActionResult Post([FromUri]ModifyTemplateAttachedFile model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;

            return Get((int)docProc.ExecuteAction(EnumDocumentActions.AddTemplateAttachedFile, cxt, model));
            
        }

        /// <summary>
        /// Изменить вложенный в шаблон файл
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put([FromBody]ModifyTemplateAttachedFile model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<ITemplateDocumentService>();

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;

            var fl = (FrontTemplateAttachedFile)docProc.ExecuteAction(EnumDocumentActions.ModifyTemplateAttachedFile, cxt, model);

            return new JsonResult(fl, this);
        }

        /// <summary>
        /// Удалить вложенный в шаблон файл
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromBody]ModifyTemplateAttachedFile model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<ITemplateDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteTemplateAttachedFile, cxt, model.Id);
            return new JsonResult(null, this);
        }
    }
}
