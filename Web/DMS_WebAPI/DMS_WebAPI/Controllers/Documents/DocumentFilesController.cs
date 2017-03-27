using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V2 + "DocumentFiles")]
    public class DocumentFilesController : ApiController
    {
        /// <summary>
        /// Получить файл документа определенной версии use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = docFileProc.GetUserFile(ctx, model);

            return new JsonResult(res, this);
        }

        /// <summary>
        /// Получить PDF копию файл документа определенной версии
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetPdfFile")]
        [HttpGet]
        public IHttpActionResult PdfFile([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = docFileProc.GetUserFilePdf(ctx, model);

            return new JsonResult(res, this);
        }

        /// <summary>
        /// Получить картинку-превью для PDF версии документа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("GetPdfPreview")]
        [HttpGet]
        public IHttpActionResult PdfPreview([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = docFileProc.GetUserFilePreview(ctx, model);

            return new JsonResult(res, this);
        }

        /// <summary>
        /// Общий список файлов don't use
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <param name="paging">paging</param>
        /// <returns></returns>
        [Route("GetFileList")]
        [HttpGet]
        public IHttpActionResult GetFileList([FromUri]FilterBase filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            if (paging == null)
            {
                paging = new UIPaging();
            }
            if (filter == null)
            {
                filter = new FilterBase();
            }
            if (filter.File == null)
            {
                filter.File = new FilterDocumentFile();
            }

            var res = new JsonResult(docFileProc.GetDocumentFiles(ctx, filter, paging), this);
            res.Paging = paging;
            return res;
        }


        /// <summary>
        /// Общий список файлов use V3
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetList")]
        [ResponseType(typeof(List<FrontDocumentAttachedFile>))]
        public IHttpActionResult PostGetList([FromBody]IncomingBase model)
        {
            if (model == null) model = new IncomingBase();
            if (model.Filter == null) model.Filter = new FilterBase();
            if (model.Filter.File == null) model.Filter.File = new FilterDocumentFile();
            if (model.Paging == null) model.Paging = new UIPaging();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = new JsonResult(docFileProc.GetDocumentFiles(ctx, model.Filter, model.Paging), this);
            res.Paging = model.Paging;
            return res;
        }

        /// <summary>
        /// Добавление нового файла use V3
        /// Если файл есть с таким именем создаться новая версия файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromUri]AddDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;
            model.IsUseMainNameFile = false;

            docProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, ctx, model);
            return GetFileList(new FilterBase { File = new FilterDocumentFile { DocumentId = new List<int> { model.DocumentId } } }, null);
        }

        /// <summary>
        /// Изменить описание(Description) файла use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(ModifyDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            var fileId = (int)docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentFile, ctx, model);

            return GetFileList(new FilterBase { File = new FilterDocumentFile { FileId = new List<int> { fileId }, IsAllDeleted = true, IsAllVersion = true } }, null);
        }

        /// <summary>
        /// Изменить имя файла use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("RenameFile")]
        public IHttpActionResult PutRenameFile(ModifyDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            docProc.ExecuteAction(EnumDocumentActions.RenameDocumentFile, ctx, model);

            return GetFileList(new FilterBase { File = new FilterDocumentFile { DocumentId = new List<int> { model.DocumentId }, OrderInDocument = new List<int> { model.OrderInDocument } } }, null);
        }

        /// <summary>
        /// Удаление основного файла use V3
        /// удаляются все версии этого файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Delete([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentFile, ctx, model);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Получение списка доступных команд по документу use V3
        /// </summary>
        /// <param name="id">ИД документа</param>
        /// <returns>Массив команд</returns>
        [Route("Actions/{id}")]
        [HttpGet]
        public IHttpActionResult Actions(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var cmdService = DmsResolver.Current.Get<ICommandService>();
            var actions = cmdService.GetDocumentFileActions(ctx, id);

            return new JsonResult(actions, this);
        }

        /// <summary>
        /// Получение списка доступных команд по файлу use V3
        /// </summary>
        /// <param name="id">ИД файлу</param>
        /// <returns>Массив команд</returns>
        [Route("ActionsByFile/{id}")]
        [HttpGet]
        public IHttpActionResult ActionsByFile(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var cmdService = DmsResolver.Current.Get<ICommandService>();
            var actions = cmdService.GetDocumentFileActions(ctx, null, id);

            return new JsonResult(actions, this);
        }

        /// <summary>
        /// Вставка версии файла к файлу use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AddUseMainNameFile")]
        [HttpPost]
        public IHttpActionResult PostAddUseMainNameFile([FromUri]AddDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;
            model.IsUseMainNameFile = true;

            docProc.ExecuteAction(EnumDocumentActions.AddDocumentFileUseMainNameFile, ctx, model);
            return GetFileList(new FilterBase { File = new FilterDocumentFile { DocumentId = new List<int> { model.DocumentId } } }, null);
        }

        /// <summary>
        /// Принять версию файла use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Accept")]
        [HttpPost]
        public IHttpActionResult PostAccept(ChangeWorkOutDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            docProc.ExecuteAction(EnumDocumentActions.AcceptDocumentFile, ctx, model);
            return GetFileList(new FilterBase { File = new FilterDocumentFile { DocumentId = new List<int> { model.DocumentId } } }, null);
        }

        /// <summary>
        /// Отклонить версию файла use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("Reject")]
        [HttpPost]
        public IHttpActionResult PostReject(ChangeWorkOutDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            docProc.ExecuteAction(EnumDocumentActions.RejectDocumentFile, ctx, model);
            return GetFileList(new FilterBase { File = new FilterDocumentFile { DocumentId = new List<int> { model.DocumentId } } }, null);
        }

        /// <summary>
        /// Сделать основной версией use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("AcceptMainVersion")]
        [HttpPost]
        public IHttpActionResult PostAcceptMainVersion(ChangeWorkOutDocumentFile model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();

            docProc.ExecuteAction(EnumDocumentActions.AcceptMainVersionDocumentFile, ctx, model);
            return GetFileList(new FilterBase { File = new FilterDocumentFile { DocumentId = new List<int> { model.DocumentId } } }, null);
        }

        /// <summary>
        /// Удаление версии файла use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteFileVersion")]
        [HttpDelete]
        public IHttpActionResult DeleteFileVersion([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentFileVersion, ctx, model);
            return new JsonResult(null, this);
        }

        /// <summary>
        /// Удаление версии файла из базы use V3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("DeleteFileVersionRecord")]
        [HttpDelete]
        public IHttpActionResult DeleteFileVersionRecord([FromUri]FilterDocumentFileIdentity model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentFileVersionRecord, ctx, model);
            return new JsonResult(null, this);
        }
    }
}
