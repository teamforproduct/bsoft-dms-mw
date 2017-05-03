using System;
using System.IO;
using System.Web.Mvc;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.ControllersV3.System
{
    [global::System.Web.Http.Authorize]
    public class ImageController : WebApiController
    {
        [HttpGet]
        public ActionResult GetFile(int clientId, int fileType, int fileId)
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFileService>();
            var fType = (EnumDocumentFileType)fileType;
            FrontDocumentAttachedFile item;
            string contentType;
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
            
            string filename = item.Name+"."+item.Extension;

            //byte[] filedata = fs.ToArray();
            //string contentType = "application/vnd.ms-excel";

            //var cd = new System.Net.Mime.ContentDisposition
            //{
            //    FileName = filename,
            //    Inline = true,
            //};

            //Response.AppendHeader("Content-Disposition", cd.ToString());

            //return File(filedata, contentType);
            return null;
        }
    }
}