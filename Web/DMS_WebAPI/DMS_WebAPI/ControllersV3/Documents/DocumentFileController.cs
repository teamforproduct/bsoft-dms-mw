using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BL.Logic.SystemServices.FileService;
using Microsoft.Ajax.Utilities;
using System.Linq;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Файлы.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentFileController : WebApiController
    {

        /// <summary>
        /// Возвращает список файлов
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [DimanicAuthorize("R")]
        [Route(Features.Files + "/Main")]
        [ResponseType(typeof(List<FrontDocumentFile>))]
        public async Task<IHttpActionResult> PostGetList([FromBody]IncomingBase model)
        {
            //var request = HttpContext.Current.Request;
            //var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            //if (appUrl != "/")
            //    appUrl = "/" + appUrl;

            //var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);
            var baseUrl = $"/{ApiPrefix.V3}";

            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                if (model == null) model = new IncomingBase();
                if (model.Filter == null) model.Filter = new FilterBase();
                if (model.Paging == null) model.Paging = new UIPaging();
                var baseurl = param.ToString();
                var docProc = DmsResolver.Current.Get<IDocumentFileService>();
                var items = docProc.GetDocumentFiles(context, model.Filter, model.Paging);
                var fileService = DmsResolver.Current.Get<IFileService>();
                items.ForEach(x =>
                {
                    x.FileLink = fileService.GetFileUri(EnumDocumentFileType.UserFile, x.Id);
                    x.PdfFileLink = fileService.GetFileUri(EnumDocumentFileType.PdfFile, x.Id);
                    x.PreviewFileLink = fileService.GetFileUri(EnumDocumentFileType.PdfPreview, x.Id);
                });
                var res = new JsonResult(items, this);
                res.Paging = model.Paging;
                return res;
            }, baseUrl);
        }

        /// <summary>
        /// Возвращает файл по ИД TODO переделать параметр
        /// </summary>
        /// <param name="Id">ИД события</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentFile))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentFileService>();
                var item = docProc.GetUserFile(context, Id);
                var res = new JsonResult(item, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает файл по ИД 
        /// </summary>
        /// <param name="Id">ИД файла</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}/Pdf")]
        [ResponseType(typeof(FrontDocumentFile))]
        public async Task<IHttpActionResult> GetPdf(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentFileService>();
                var item = docProc.GetUserFilePdf(context, Id);
                var res = new JsonResult(item, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает файл по ИД 
        /// </summary>
        /// <param name="Id">ИД файла</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}/Preview")]
        [ResponseType(typeof(FrontDocumentFile))]
        public async Task<IHttpActionResult> GetPreview(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentFileService>();
                var item = docProc.GetUserFilePreview(context, Id);
                var res = new JsonResult(item, this);
                return res;
            });
        }

        /// <summary>
        /// Добавляет файл, в зависимости от параметров новый или версию существующего
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Files)]
        public async Task<IHttpActionResult> Post([FromBody]List<AddDocumentFile> model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDocumentActions.AddDocumentFile, model, model.Select(x => x.CurrentPositionId).FirstOrDefault());
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Измененяет описание файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/ChangeDescription")]
        public async Task<IHttpActionResult> Put([FromBody]ModifyDocumentFile model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDocumentActions.ModifyDocumentFile, model, model.CurrentPositionId);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Измененяет  имя файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/Rename")]
        public async Task<IHttpActionResult> RenameFile([FromBody]ModifyDocumentFile model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDocumentActions.RenameDocumentFile, model, model.CurrentPositionId);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }


        /// <summary>
        /// Принимает версию файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/AcceptVersion")]
        public async Task<IHttpActionResult> AcceptVersion([FromBody]ChangeWorkOutDocumentFile model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.AcceptDocumentFile, model);
                var res = new JsonResult(true, this);
                return res;
            });
        }

        /// <summary>
        /// Делает версию файла основной
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/AcceptMainVersion")]
        public async Task<IHttpActionResult> AcceptMainVersion([FromBody]ChangeWorkOutDocumentFile model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.AcceptMainVersionDocumentFile, model);
                var res = new JsonResult(true, this);
                return res;
            });
        }

        /// <summary>
        /// Отклоняет версию файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/RejectVersion")]
        public async Task<IHttpActionResult> RejectVersion([FromBody]ChangeWorkOutDocumentFile model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.RejectDocumentFile, model);
                var res = new JsonResult(true, this);
                return res;
            });
        }



        /// <summary>
        /// Удаляет файл (удадяет все версии) (ставится отметка или полное удаление)
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Files + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.DeleteDocumentFile, Id);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Удаляет версию файла (ставится отметка)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Files + "/DeleteFileVersion")]
        public async Task<IHttpActionResult> DeleteFileVersion([FromUri]FilterDocumentFileIdentity model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.DeleteDocumentFileVersion, model);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Удаляет версию файла (окончательное удаление)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Files + "/DeleteFileVersionRecord")]
        public async Task<IHttpActionResult> DeleteFileVersionRecord([FromUri]FilterDocumentFileIdentity model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.DeleteDocumentFileVersionRecord, model);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает меню по ИД документа для работы с файлами 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Files + "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public async Task<IHttpActionResult> ActionsByDocument([FromUri]int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<ICommandService>();
                var items = docProc.GetDocumentFileActions(context, Id);
                var res = new JsonResult(items, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает меню по ИД файла для работы с файлами 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}" + "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public async Task<IHttpActionResult> ActionsByFile([FromUri]int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<ICommandService>();
                var items = docProc.GetDocumentFileActions(context, null, Id);
                var res = new JsonResult(items, this);
                return res;
            });
        }
    }
}
