using System;
using System.Web.Mvc;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemServices.FileService;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.ControllersV3.System
{
    [Authorize]
    [RoutePrefix(ApiPrefix.V3)]
    public class ImageController : Controller
    {
        [HttpGet]
        [Route("files/{clientId}/{fileType}/{fileId}")]
        public ActionResult GetFile(int clientId, int fileType, int fileId)
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFileService>();
            var fileSrv = DmsResolver.Current.Get<IFileService>();
            var fType = (EnumDocumentFileType)fileType;
            FrontDocumentFile item;
            
            switch (fType)
            {
                case EnumDocumentFileType.UserFile:
                    item = docProc.GetUserFile(context, fileId);
                    break;
                case EnumDocumentFileType.PdfFile:
                    item = docProc.GetUserFilePdf(context, fileId);
                    break;
                case EnumDocumentFileType.PdfPreview:
                    item = docProc.GetUserFilePreview(context, fileId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            string filename = item.File.FileName;
            string contentType = fileSrv.GetMimetype(item.File.Extension);

            var cd = new global::System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(item.File.FileContent, contentType);

        }
    }
}