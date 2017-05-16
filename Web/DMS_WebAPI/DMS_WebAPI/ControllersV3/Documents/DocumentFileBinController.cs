using System.Threading.Tasks;
using System.Web.Mvc;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.FileService;
using BL.Model.Enums;
using BL.Model.Exception;
using DMS_WebAPI.Utilities;
using BL.Model.SystemCore;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentFileBinController : Controller
    {
        /// <summary>
        /// Возвращает сам файл
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="CannotAccessToFile"></exception>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}/File")]
        public async Task<ActionResult> GetUserFile(int Id)
        {
            return await GetFile(EnumDocumentFileType.UserFile, Id);
        }

        /// <summary>
        /// Возвращает pdf 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="CannotAccessToFile"></exception>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}/Pdf")]
        public async Task<ActionResult> GetPdfFile(int Id)
        {
            return await GetFile(EnumDocumentFileType.PdfFile, Id);
        }

        /// <summary>
        /// Возвращает миниатюру
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="CannotAccessToFile"></exception>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}/Preview")]
        public async Task<ActionResult> GetPdfPreview(int Id)
        {
            return await GetFile(EnumDocumentFileType.PdfPreview, Id);
        }

        private async Task<ActionResult> GetFile(EnumDocumentFileType type, int Id)
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();

            var fileSrv = DmsResolver.Current.Get<IFileService>();
            var item = await fileSrv.GetFile(context, type, Id);

            string contentType = fileSrv.GetMimetype(item.File.Extension);

            var cd = new global::System.Net.Mime.ContentDisposition
            {
                FileName = item.File.FileName,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(item.File.FileContent, contentType);
        }
    }
}