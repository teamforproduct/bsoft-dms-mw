﻿using System.Threading.Tasks;
using System.Web.Mvc;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.FileService;
using BL.Model.Enums;
using BL.Model.Exception;
using DMS_WebAPI.Utilities;
using BL.Model.SystemCore;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class ImageController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="CannotAccessToFile"></exception>
        [HttpGet]
        [Route(Features.Attachments + "/UserFile/{Id}")]
        public async Task<ActionResult> GetUserFile(int Id)
        {
            return await GetFile(EnumDocumentFileType.UserFile, Id);
        }

        [HttpGet]
        [Route(Features.Attachments + "/PdfFile/{Id}")]
        public async Task<ActionResult> GetPdfFile(int Id)
        {
            return await GetFile(EnumDocumentFileType.PdfFile, Id);
        }

        [HttpGet]
        [Route(Features.Attachments + "/PdfPreview/{Id}")]
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