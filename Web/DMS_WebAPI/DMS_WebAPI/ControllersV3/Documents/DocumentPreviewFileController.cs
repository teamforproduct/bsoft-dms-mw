using System.Web.Http;
using System.Web.Http.Description;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.ControllersV3.Documents
{   
    /// <summary>
     /// Документы. Файлы.
     /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentPreviewFileController : ApiController
    {
        /// <summary>
        /// Возвращает файл по ИД 
        /// </summary>
        /// <param name="Id">ИД файла</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route(Features.FilePreview + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentAttachedFile))]
        [DimanicAuthorize("R")]
        public IHttpActionResult Get([FromUri]FilterDocumentFileIdentity Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFileService>();
            var item = docProc.GetUserFilePreview(ctx, Id);
            var res = new JsonResult(item, this);
            return res;
        }
    }
}