using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore;
using BL.Model.Common;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;

namespace DMS_WebAPI.ControllersV3.DocumentTemplates
{
    /// <summary>
    /// Шаблоны документов. Файлы.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Templates)]
    public class TemplateFilesController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список файлов
        /// </summary>
        /// <param name="Id">ИД шаблона</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Files)]
        [ResponseType(typeof(List<FrontTemplateAttachedFile>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterTemplateAttachedFile filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterTemplateAttachedFile();
            filter.TemplateId =  Id ;

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItems = tmpService.GetTemplateAttachedFiles(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Возвращает файл по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}")]
        [ResponseType(typeof(FrontTemplateAttachedFile))]
        public IHttpActionResult Get(int Id)
        {
            //TODO PDF
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = tmpService.GetTemplateAttachedFile(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет файл
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Files)]
        public IHttpActionResult Post([FromUri]AddTemplateAttachedFile model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;

            var tmpItem = Action.Execute(EnumDocumentActions.AddTemplateAttachedFile, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует файл
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files)]
        public IHttpActionResult Put([FromBody]ModifyTemplateAttachedFile model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            //if (HttpContext.Current.Request.Files.Count > 0)
            //{
            //    HttpPostedFile file = HttpContext.Current.Request.Files[0];
            //    model.PostedFileData = file;
            //    model.FileName = file.FileName;
            //    model.FileType = file.ContentType;
            //}
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = (FrontTemplateAttachedFile)tmpService.ExecuteAction(EnumDocumentActions.ModifyTemplateAttachedFile, ctx, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Удаляет файл
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Files + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteTemplateAttachedFile, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
