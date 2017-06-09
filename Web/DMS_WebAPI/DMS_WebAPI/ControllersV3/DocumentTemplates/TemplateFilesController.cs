using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemServices.TempStorage;
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
    public class TemplateFilesController : WebApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            //TODO PDF
            var tmpService = DmsResolver.Current.Get<ITemplateService>();
            var tmpItem = tmpService.GetTemplateFile(context, Id);
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
        [ResponseType(typeof (List<FrontTemplateFile>))]
        public async Task<IHttpActionResult> Get(int Id, [FromUri] FilterTemplateFile filter)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                if (filter == null) filter = new FilterTemplateFile();
                filter.TemplateId = Id;

                var tmpService = DmsResolver.Current.Get<ITemplateService>();
                var tmpItems = tmpService.GetTemplateFiles(context, filter);
                var res = new JsonResult(tmpItems, this);
                return res;
            });
        }


        /// <summary>
        /// Возвращает файл по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}")]
        [ResponseType(typeof (FrontTemplateFile))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
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
        public async Task<IHttpActionResult> Post([FromBody] AddTemplateFile model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.ExecuteDocumentAction(context, EnumActions.AddTemplateFile, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует файл
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files)]
        public async Task<IHttpActionResult> Put([FromBody] ModifyTemplateFile model)
        {
            //if (HttpContext.Current.Request.Files.Count > 0)
            //{
            //    HttpPostedFile file = HttpContext.Current.Request.Files[0];
            //    model.PostedFileData = file;
            //    model.FileName = file.FileName;
            //    model.FileType = file.ContentType;
            //}
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpService = DmsResolver.Current.Get<IDocumentService>();
                var tmpItem =
                    (FrontTemplateFile)
                        tmpService.ExecuteAction(EnumActions.ModifyTemplateFile, context, model);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
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
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.ExecuteDocumentAction(context, EnumActions.DeleteTemplateFile, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

    }
}
