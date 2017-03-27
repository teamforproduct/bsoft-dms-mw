using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Common;
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
        private IHttpActionResult GetById(IContext context, int Id)
        {
            //TODO PDF
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = tmpService.GetTemplateAttachedFile(context, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает список файлов
        /// </summary>
        /// <param name="Id">ИД шаблона</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Files)]
        [ResponseType(typeof(List<FrontTemplateAttachedFile>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterTemplateAttachedFile filter)
        {
            if (filter == null) filter = new FilterTemplateAttachedFile();
            filter.TemplateId =  Id ;

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItems = tmpService.GetTemplateAttachedFiles(ctx, filter);
            var res = new JsonResult(tmpItems, this);
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
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Добавляет файл
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Files)]
        public async Task<IHttpActionResult> Post([FromUri]AddTemplateAttachedFile model)
        {
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;

            var tmpItem = Action.Execute(EnumDocumentActions.AddTemplateAttachedFile, model);
            return GetById(context, tmpItem);
        }

        /// <summary>
        /// Корректирует файл
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyTemplateAttachedFile model)
        {
            //if (HttpContext.Current.Request.Files.Count > 0)
            //{
            //    HttpPostedFile file = HttpContext.Current.Request.Files[0];
            //    model.PostedFileData = file;
            //    model.FileName = file.FileName;
            //    model.FileType = file.ContentType;
            //}
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = (FrontTemplateAttachedFile)tmpService.ExecuteAction(EnumDocumentActions.ModifyTemplateAttachedFile, ctx, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Удаляет файл
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Files + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete([FromUri] int Id)
        {
            Action.Execute(EnumDocumentActions.DeleteTemplateAttachedFile, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
