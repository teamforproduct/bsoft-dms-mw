using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
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
    /// Документы
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Document)]
    public class DocumentInfoController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список документов
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontDocument>))]
        public IHttpActionResult PostGetList([FromBody]IncomingBase model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (model == null) model = new IncomingBase();
            if (model.Filter == null) model.Filter = new FilterBase();
            if (model.Paging == null) model.Paging = new UIPaging();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocuments(ctx, model.Filter, model.Paging);
            var res = new JsonResult(items, this);
            res.Paging = model.Paging;
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает документ по ИД TODO убрать вложенные элементы?
        /// </summary>
        /// <param name="Id">ИД Документа</param>
        /// <returns>Документ</returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDocument))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocument(ctx, Id);
            var metaData = docProc.GetModifyMetaData(ctx, item);
            var res = new JsonResult(item, metaData, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает отчет Регистрационная карточка документа
        /// </summary>
        /// <param name="Id">ИД Документа</param>
        /// <returns>Документ</returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}/ReportRegistrationCardDocument")]
        [ResponseType(typeof(FrontReport))]
        public IHttpActionResult GetReportRegistrationCardDocument(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = docProc.ExecuteAction(EnumDocumentActions.ReportRegistrationCardDocument, ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет документ по шаблону
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public IHttpActionResult Post([FromBody]AddDocumentByTemplateDocument model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.AddDocument, model, model.CurrentPositionId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Корректирует документ TODO Required!!!!!!!!!!!!!!!!!!
        /// </summary>
        /// <param name="model">Модель для обновления документа</param>
        /// <returns>Обновленный документ</returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyDocument model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.ModifyDocument, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Удаляет документ
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public IHttpActionResult Delete(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteDocument, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Копирует документ 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Добавленный документ</returns>
        [Route(Features.Info + "/Duplicate")]
        [HttpPost]
        public IHttpActionResult CopyDocument([FromBody]CopyDocument model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.CopyDocument, model, model.CurrentPositionId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Регистрирует проект
        /// Возможности:
        /// 1. Получить регистрационный номер
        /// 2. Зарегистрировать документ
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route(Features.Info + "/Register")]
        [HttpPost]
        public IHttpActionResult RegisterDocument(RegisterDocument model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.RegisterDocument, model, model.CurrentPositionId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Получает следующий регистрационный номер
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route(Features.Info + "/GetNextRegisterDocumentNumber")]
        [HttpPost]
        [ResponseType(typeof(FrontRegistrationFullNumber))]
        public IHttpActionResult GetNextRegisterDocumentNumber(RegisterDocument model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = docProc.GetNextRegisterDocumentNumber(ctx, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает меню для работы с документом 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}"+"/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public IHttpActionResult Actions([FromUri]int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<ICommandService>();
            var tmpItem = docProc.GetDocumentActions(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
