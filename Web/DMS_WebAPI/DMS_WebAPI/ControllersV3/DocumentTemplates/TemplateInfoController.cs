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
    /// Шаблоны документов
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + ApiPrefix.Templates)]
    public class TemplateInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список шаблонов документов
        /// </summary>
        /// <param name="ftSearch"></param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/Main")]
        [ResponseType(typeof(List<FrontMainTemplateDocument>))]
        public IHttpActionResult Get([FromUri]FullTextSearch ftSearch, [FromUri]FilterTemplateDocument filter, UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItems = tmpService.GetMainTemplateDocument(ctx, ftSearch, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Возвращает шаблон по Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Info/{Id:int}")]
        [ResponseType(typeof(FrontTemplateDocument))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<ITemplateDocumentService>();
            var tmpItem = tmpService.GetTemplateDocument(ctx, Id);
            var metaData = tmpService.GetModifyMetaData(ctx, tmpItem);
            var res = new JsonResult(tmpItem, metaData, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет шаблон
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Info")]
        public IHttpActionResult Post([FromBody]ModifyTemplateDocument model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.AddTemplateDocument, model);
            return Get(tmpItem);
        }

        /// <summary>
        /// Создает копию шаблон
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Info/Duplicate")]
        public IHttpActionResult Duplicate([FromBody]int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.CopyTemplateDocument, Id);
            return Get(tmpItem);
        }

        /// <summary>
        /// Корректирует шаблон
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Info")]
        public IHttpActionResult Put([FromBody]ModifyTemplateDocument model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.ModifyTemplateDocument, model);
            return Get(model.Id); // TODO - почему Id nullable?
        }

        /// <summary>
        /// Удаляет шаблон
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Info/{Id:int}")]
        public IHttpActionResult Delete([FromUri] int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteTemplateDocument, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
