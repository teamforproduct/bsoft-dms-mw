using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.SystemCore;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    [RoutePrefix("api/v2/DocumentFiles")]
    public class DocumentFilesController : ApiController
    {
        //GET: api/Files
        public IHttpActionResult Get([FromUri]FilterDocumentFileIdentity model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = docFileProc.GetUserFile(cxt, model);

            return new JsonResult(res, this);
        }

        // GET: api/Files/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            return new JsonResult(docFileProc.GetDocumentFiles(cxt,id), this);
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docFileProc = DmsResolver.Current.Get<IDocumentFileService>();
            var res = new JsonResult(docFileProc.GetDocumentFiles(cxt, filter, paging), this);
            res.Paging = paging;
            return res;
        }
        
        // POST: api/Files
        public IHttpActionResult Post([FromBody]ModifyDocumentFiles model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.AddDocumentFile, cxt, model);
            return Get(model.DocumentId);
        }

        // PUT: api/Files/5
        public IHttpActionResult Put([FromBody]ModifyDocumentFile model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var fileId = (int)docProc.ExecuteAction(EnumDocumentActions.ModifyDocumentFile, cxt, model);

            return GetFileList(new FilterDocumentAttachedFile { AttachedFileId = new List<int> { fileId } }, null);
        }

        // DELETE: api/Files
        public IHttpActionResult Delete([FromBody]FilterDocumentFileIdentity model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            docProc.ExecuteAction(EnumDocumentActions.DeleteDocumentFile, cxt, model);
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
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var cmdService = DmsResolver.Current.Get<ICommandService>();
            var actions = cmdService.GetDocumentFileActions(cxt, id);

            return new JsonResult(actions, this);
        }
    }
}
