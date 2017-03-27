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
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.PaperList)]
    public class PaperListInfoController : ApiController
    {
        /// <summary>
        /// Возвращает список реестров БН
        /// </summary>
        /// <param name="ftSearch">Фильтр</param>
        /// <param name="filter">Фильтр</param>
        /// <param name="paging">Пейджинг</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontDocumentPaperList>))]
        public IHttpActionResult Get([FromUri]FullTextSearch ftSearch, [FromUri] FilterDocumentPaperList filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetMainDocumentPaperLists(ctx, ftSearch, filter, paging);
            var res = new JsonResult(items, this);
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
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocumentPaperList(ctx, Id);
            var res = new JsonResult(item, this);
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
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = docProc.ExecuteAction(EnumDocumentActions.ReportRegisterTransmissionDocuments, ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Добавляет реестр БН
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddDocumentPaperList model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = (List<int>)docProc.ExecuteAction(EnumDocumentActions.AddDocumentPaperList, ctx, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Измененяет реестр БН
        /// </summary>
        /// <param name="model">Модель для обновления</param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyDocumentPaperList model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.ModifyDocumentPaperList, model);
            var res = new JsonResult(tmpItem, this);
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
            Action.Execute(EnumDocumentActions.DeleteDocumentPaperList, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Отменяет планирование движения бумажного носителя
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [Route(Features.Info + "/CancelPlan")]
        [HttpPut]
        public IHttpActionResult CancelPlanDocumentPaperEvent([FromBody]int Id)
        {
            Action.Execute(EnumDocumentActions.CancelPlanDocumentPaperEvent, new PaperList { PaperListId = Id });
            var res = new JsonResult(null, this);
            return res;
        }

        /// <summary>
        /// Отмечает передачу бумажного носителя
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [Route(Features.Info + "/Send")]
        [HttpPut]
        public IHttpActionResult SendDocumentPaperEvent([FromBody]int Id)
        {
            Action.Execute(EnumDocumentActions.SendDocumentPaperEvent, new PaperList { PaperListId = Id });
            var res = new JsonResult(null, this);
            return res;
        }

        /// <summary>
        ///  Отменяет передачу бумажного носителя
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [Route(Features.Info + "/CancelSend")]
        [HttpPut]
        public IHttpActionResult CancelSendDocumentPaperEvent([FromBody]int Id)
        {
            Action.Execute(EnumDocumentActions.CancelSendDocumentPaperEvent, new PaperList { PaperListId = Id });
            var res = new JsonResult(null, this);
            return res;
        }

        /// <summary>
        /// Отмечает прием бумажного носителя
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [Route(Features.Info + "/Recieve")]
        [HttpPut]
        public IHttpActionResult RecieveDocumentPaperEvent([FromBody]int Id)
        {
            Action.Execute(EnumDocumentActions.RecieveDocumentPaperEvent, new PaperList { PaperListId = Id });
            var res = new JsonResult(null, this);
            return res;
        }

    }
}
