using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.SystemServices.AutoPlan;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Actions;
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

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Бумажные носители.
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Document)]
    public class DocumentPaperController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список бумажных носителей
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="paging">Пейджинг</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Papers)]
        [ResponseType(typeof(List<FrontDocumentPaper>))]
        public IHttpActionResult Get([FromUri]FilterDocumentPaper filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocumentPapers(ctx, filter, paging);
            var res = new JsonResult(items, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает бумажный носитель по ИД
        /// </summary>
        /// <param name="Id">ИД БН</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Papers + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentPaper))]
        public IHttpActionResult GetById(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocumentPaper(ctx, Id);
            var res = new JsonResult(item, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет бумажный носитель
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Papers)]
        public IHttpActionResult Post([FromBody]AddDocumentPapers model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.AddDocumentPaper, model, model.CurrentPositionId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Измененяет бумажный носитель
        /// </summary>
        /// <param name="model">Модель для обновления</param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Papers)]
        public IHttpActionResult Put([FromBody]ModifyDocumentPapers model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.ModifyDocumentPaper, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Удаляет бумажный носитель
        /// </summary>
        /// <param name="Id">ИД БН</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Papers + "/{Id:int}")]
        public IHttpActionResult Delete(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteDocumentPaper, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает меню по ИД документа для работы с БН 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Papers + "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public IHttpActionResult Actions([FromUri]int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<ICommandService>();
            var items = docProc.GetDocumentPaperActions(ctx, Id);
            var res = new JsonResult(items, this);
            res.SpentTime = stopWatch;
            return res;
        }


    }
}
