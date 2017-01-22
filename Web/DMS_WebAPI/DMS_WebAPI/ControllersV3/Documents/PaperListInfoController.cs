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
using BL.Model.Reports.FrontModel;
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
    /// Документы. Сохраненные фильтры документов.
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.PaperList)]
    public class PaperListInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список реестров БН
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="paging">Пейджинг</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontDocumentPaperList>))]
        public IHttpActionResult Get([FromUri] FilterDocumentPaperList filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocumentPaperLists(ctx, filter, paging);
            var res = new JsonResult(items, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает реестр БН по ИД
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentPaperList))]
        public IHttpActionResult GetById(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocumentPaperList(ctx, Id);
            var res = new JsonResult(item, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает отчет Реестр передачи документов
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}/ReportRegisterTransmissionDocuments")]
        [ResponseType(typeof(FrontReport))]
        public IHttpActionResult GetReportRegisterTransmissionDocuments(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = docProc.ExecuteAction(EnumDocumentActions.ReportRegisterTransmissionDocuments, ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет реестр БН
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddDocumentPaperLists model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = (List<int>)docProc.ExecuteAction(EnumDocumentActions.AddDocumentPaperList, ctx, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Измененяет реестр БН
        /// </summary>
        /// <param name="model">Модель для обновления</param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyDocumentPaperLists model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.ModifyDocumentPaperList, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Удаляет реестр БН
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteDocumentPaperList, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }
        
    }
}
