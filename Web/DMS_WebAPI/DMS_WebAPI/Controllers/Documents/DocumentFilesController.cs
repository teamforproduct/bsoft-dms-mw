using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.SystemCore;
using System.Web;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix("api/v2/DocumentFiles")]
    public class DocumentFilesController : ApiController
    {
        /// <summary>
        /// Получить файл документа определенной версии
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
            if (paging == null)
            {
                paging = new UIPaging();
            }

            if (filter == null)
            {
                filter = new FilterDocumentAttachedFile();
            }
            var res = new JsonResult(docFileProc.GetDocumentFiles(ctx, filter, paging), this);
            res.Paging = paging;
            return res;
        }

        /// <summary>
        /// Добавление нового файла
        /// Если файл есть с таким именем создаться новая версия файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromUri]AddDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;
            model.IsUseMainNameFile = false;


            docProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, ctx, model);
            return GetFileList(new FilterDocumentAttachedFile { DocumentId = new List<int> { model.DocumentId } }, null);
        }

        /// <summary>
        /// Изменить описание(Description) файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(ModifyDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var fileId = (int)docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentFile, ctx, model);

            return GetFileList(new FilterDocumentAttachedFile { AttachedFileId = new List<int> { fileId }, IsAllDeleted = true, IsAllVersion = true }, null);
        }

        /// <summary>
        /// Изменить имя файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RenameFile")]
        public IHttpActionResult PutRenameFile(ModifyDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            docProc.ExecuteAction(EnumDocumentActions.RenameDocumentFile, ctx, model);

            return GetFileList(new FilterDocumentAttachedFile { DocumentId = new List<int> { model.DocumentId }, OrderInDocument = new List<int> { model.OrderInDocument }}, null);
        }

        /// <summary>
        /// Удаление основного файла
        /// удаляются все версии этого файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Вставка версии файла к файлу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUseMainNameFile")]
        [HttpPost]
        public IHttpActionResult PostAddUseMainNameFile([FromUri]AddDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;
            model.IsUseMainNameFile = true;

            docProc.ExecuteAction(EnumDocumentActions.AddDocumentFileUseMainNameFile, ctx, model);
            return GetFileList(new FilterDocumentAttachedFile { DocumentId = new List<int> { model.DocumentId } }, null);
        }

        /// <summary>
        /// Принять версию файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Accept")]
        [HttpPost]
        public IHttpActionResult PostAccept(ChangeWorkOutDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            docProc.ExecuteAction(EnumDocumentActions.AcceptDocumentFile, ctx, model);
            return GetFileList(new FilterDocumentAttachedFile { DocumentId = new List<int> { model.DocumentId } }, null);
        }

        /// <summary>
        /// Отклонить версию файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Reject")]
        [HttpPost]
        public IHttpActionResult PostReject(ChangeWorkOutDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            docProc.ExecuteAction(EnumDocumentActions.RejectDocumentFile, ctx, model);
            return GetFileList(new FilterDocumentAttachedFile { DocumentId = new List<int> { model.DocumentId } }, null);
        }

        /// <summary>
        /// Удаление версии файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteFileVersion")]
        [HttpDelete]
        public IHttpActionResult DeleteFileVersion([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentFileVersion, ctx, model);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Удаление версии файла из базы
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteFileVersionRecord")]
        [HttpDelete]
        public IHttpActionResult DeleteFileVersionRecord([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentFileVersionRecord, ctx, model);
            return new JsonResult(null, this);
        }
    }
}
