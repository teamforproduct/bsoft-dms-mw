using System.Threading.Tasks;
using System.Web.Mvc;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.SystemServices.FileService;
using BL.Model.Enums;
using BL.Model.Exception;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.ControllersV3.System
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix("api/v3")]
    public class ImageController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="fileType"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        /// <exception cref="CannotAccessToFile"></exception>
        [HttpGet]
        [Route("files/{clientId}/{fileType}/{fileId}")]
        public async Task<ActionResult> GetFile(int clientId, int fileType, int fileId)
        {
            var context = DmsResolver.Current.Get<UserContexts>().Get();

            if (context.Client.Id!=clientId) throw new CannotAccessToFile();

            var fileSrv = DmsResolver.Current.Get<IFileService>();
            var item = await fileSrv.GetFile(context, (EnumDocumentFileType)fileType, fileId);

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