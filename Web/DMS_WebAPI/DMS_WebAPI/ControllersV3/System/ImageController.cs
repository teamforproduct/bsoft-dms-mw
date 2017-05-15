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
        /// <param name="FileType"></param>
        /// <param name="FileId"></param>
        /// <returns></returns>
        /// <exception cref="CannotAccessToFile"></exception>
        [HttpGet]
        [Route(Features.Attachments + "/{FileType}/{FileId}")]
        public async Task<ActionResult> GetFile(EnumDocumentFileType FileType, int FileId)
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();

            var fileSrv = DmsResolver.Current.Get<IFileService>();
            var item = await fileSrv.GetFile(context, (EnumDocumentFileType)FileType, FileId);

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