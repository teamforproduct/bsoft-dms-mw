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
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.DocumentTemplates
{
    /// <summary>
    /// Шаблоны документов. Бумажные носители.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Templates)]
    public class TemplatePapersController : ApiController
    {
        /// <summary>
        /// Возвращает список бумажных носителей
        /// </summary>
        /// <param name="Id">ИД шаблона</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Papers)]
        [ResponseType(typeof(List<FrontTemplateDocumentPaper>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterTemplateDocumentPaper filter)
        {
            if (filter == null) filter = new FilterTemplateDocumentPaper();
            filter.TemplateId =  Id ;

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItems = tmpService.GetTemplateDocumentPapers(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }


        /// <summary>
        /// Возвращает бумажный носитель по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Papers + "/{Id:int}")]
        [ResponseType(typeof(FrontTemplateDocumentPaper))]
        public IHttpActionResult Get(int Id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = tmpService.GetTemplateDocumentPaper(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Добавляет бумажный носитель
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Papers)]
        public IHttpActionResult Post([FromBody]AddTemplateDocumentPaper model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.AddTemplateDocumentPaper, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует бумажный носитель
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Papers)]
        public IHttpActionResult Put([FromBody]ModifyTemplateDocumentPaper model)
        {
            Action.Execute(EnumDocumentActions.ModifyTemplateDocumentPaper, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет бумажный носитель
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Papers + "/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            Action.Execute(EnumDocumentActions.DeleteTemplateDocumentPaper, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

    }
}
