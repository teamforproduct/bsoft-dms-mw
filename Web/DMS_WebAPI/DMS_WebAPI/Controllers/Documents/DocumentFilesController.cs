using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.SystemCore;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix("api/v2/DocumentFiles")]
    public class DocumentFilesController : ApiController
    {
        //GET: api/Files
        public IHttpActionResult Get([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = docFileProc.GetUserFile(ctx, model);

            return new JsonResult(res, this);
        }

        /// <summary>
        /// Общий список файлов
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns></returns>
        [Route("GetFileList")]
        [HttpGet]
        public IHttpActionResult GetFileList([FromUri]FilterDocumentAttachedFile filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = new JsonResult(docFileProc.GetDocumentFiles(ctx, filter, paging), this);
            res.Paging = paging;
            return res;
        }

        public IHttpActionResult Post([FromUri]ModifyDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;


            docProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, ctx, model);
            return GetFileList(new FilterDocumentAttachedFile { DocumentId = new List<int> { model.DocumentId } }, null);
        }

        // PUT: api/Files/5
        public IHttpActionResult Put([FromUri]ModifyDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;

            var fileId = (int)docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentFile, ctx, model);

            return GetFileList(new FilterDocumentAttachedFile { AttachedFileId = new List<int> { fileId } }, null);
        }

        // DELETE: api/Files
        public IHttpActionResult Delete([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentFile, ctx, model);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Получение списка доступных команд по документу
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns>Массив команд</returns>
        [Route("Actions/{id}")]
        [HttpGet]
        public IHttpActionResult Actions(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var cmdService = DmsResolver.Current.Get<ICommandService>();
            var actions = cmdService.GetDocumentFileActions(ctx, id);

            return new JsonResult(actions, this);
        }
    }
}
