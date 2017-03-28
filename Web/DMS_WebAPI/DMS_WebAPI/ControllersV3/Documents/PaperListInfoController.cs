using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Common;
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
using System.Threading.Tasks;
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
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocumentPaperList(context, Id);
            var res = new JsonResult(item, this);
            return res;
        }

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
        public async Task<IHttpActionResult> Get([FromUri]FullTextSearch ftSearch, [FromUri] FilterDocumentPaperList filter, [FromUri]UIPaging paging)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentService>();
                   var items = docProc.GetMainDocumentPaperLists(context, ftSearch, filter, paging);
                   var res = new JsonResult(items, this);
                   return res;
               });
        }

        /// <summary>
        /// Возвращает реестр БН по ИД
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentPaperList))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                return GetById(context, Id);
            });
        }

        /// <summary>
        /// Возвращает отчет Реестр передачи документов
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}/ReportRegisterTransmissionDocuments")]
        [ResponseType(typeof(FrontReport))]
        public async Task<IHttpActionResult> GetReportRegisterTransmissionDocuments(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentService>();
                   var tmpItem = docProc.ExecuteAction(EnumDocumentActions.ReportRegisterTransmissionDocuments, context, Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Добавляет реестр БН
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Post([FromBody]AddDocumentPaperList model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var docProc = DmsResolver.Current.Get<IDocumentService>();
                   var tmpItem = (List<int>)docProc.ExecuteAction(EnumDocumentActions.AddDocumentPaperList, context, model);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Измененяет реестр БН
        /// </summary>
        /// <param name="model">Модель для обновления</param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Put([FromBody]ModifyDocumentPaperList model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   var tmpItem = Action.Execute(context, EnumDocumentActions.ModifyDocumentPaperList, model);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Удаляет реестр БН
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.DeleteDocumentPaperList, Id);
                   var tmpItem = new FrontDeleteModel(Id);
                   var res = new JsonResult(tmpItem, this);
                   return res;
               });
        }

        /// <summary>
        /// Отменяет планирование движения бумажного носителя
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [Route(Features.Info + "/CancelPlan")]
        [HttpPut]
        public async Task<IHttpActionResult> CancelPlanDocumentPaperEvent([FromBody]int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.CancelPlanDocumentPaperEvent, new PaperList { PaperListId = Id });
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Отмечает передачу бумажного носителя
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [Route(Features.Info + "/Send")]
        [HttpPut]
        public async Task<IHttpActionResult> SendDocumentPaperEvent([FromBody]int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.SendDocumentPaperEvent, new PaperList { PaperListId = Id });
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        ///  Отменяет передачу бумажного носителя
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [Route(Features.Info + "/CancelSend")]
        [HttpPut]
        public async Task<IHttpActionResult> CancelSendDocumentPaperEvent([FromBody]int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.CancelSendDocumentPaperEvent, new PaperList { PaperListId = Id });
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

        /// <summary>
        /// Отмечает прием бумажного носителя
        /// </summary>
        /// <param name="Id">ИД реестра БН</param>
        /// <returns></returns>
        [Route(Features.Info + "/Recieve")]
        [HttpPut]
        public async Task<IHttpActionResult> RecieveDocumentPaperEvent([FromBody]int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
               {
                   Action.Execute(context, EnumDocumentActions.RecieveDocumentPaperEvent, new PaperList { PaperListId = Id });
                   var res = new JsonResult(null, this);
                   return res;
               });
        }

    }
}
