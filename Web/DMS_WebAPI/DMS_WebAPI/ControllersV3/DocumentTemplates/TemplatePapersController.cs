using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.DocumentTemlates
{
    /// <summary>
    /// Шаблоны документов. Бумажные носители.
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Templates)]
    public class TemplatePapersController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список бумажных носителей
        /// </summary>
        /// <param name="Id">ИД шаблона</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/Papers")]
        [ResponseType(typeof(List<FrontTemplateDocumentPaper>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterTemplateDocumentPaper filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterTemplateDocumentPaper();
            filter.TemplateId =  Id ;

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItems = tmpService.GetTemplateDocumentPapers(ctx, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Возвращает бумажный носитель по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Papers/{Id:int}")]
        [ResponseType(typeof(FrontTemplateDocumentPaper))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = tmpService.GetTemplateDocumentPaper(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет бумажный носитель
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Papers")]
        public IHttpActionResult Post([FromBody]AddTemplateDocumentPaper model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.AddTemplateDocumentPaper, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует бумажный носитель
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Papers")]
        public IHttpActionResult Put([FromBody]ModifyTemplateDocumentPaper model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.ModifyTemplateDocumentPaper, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет бумажный носитель
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Papers/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteTemplateDocumentPaper, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
