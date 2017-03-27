using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Logic.EncryptionCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Actions;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Reports.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentInfoController : ApiController
    {
        private IHttpActionResult GetById(IContext context, int Id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocument(context, Id);
            var metaData = docProc.GetModifyMetaData(context, item);
            var res = new JsonResult(item, metaData, this);
            return res;
        }

        /// <summary>
        /// Возвращает список документов
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [DimanicAuthorize("R")]
        [Route(Features.Info + "/Main")]
        [ResponseType(typeof(List<FrontDocument>))]
        public IHttpActionResult PostGetList([FromBody]IncomingBase model)
        {
            if (model == null) model = new IncomingBase();
            if (model.Filter == null) model.Filter = new FilterBase();
            if (model.Paging == null) model.Paging = new UIPaging();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocuments(ctx, model.Filter, model.Paging);
            var res = new JsonResult(items, this);
            res.Paging = model.Paging;
            return res;
        }

        /// <summary>
        /// Возвращает список тегов с количеством документов по фильтру
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [DimanicAuthorize("R")]
        [Route(Features.Info + "/GroupCountTags")]
        [ResponseType(typeof(List<FrontDocumentTag>))]
        public IHttpActionResult PostGetGroupCountTags([FromBody]FilterBase model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocuments(ctx, model ?? new FilterBase(), new UIPaging(), EnumGroupCountType.Tags).ToList().FirstOrDefault()?.DocumentTags; ;
            var res = new JsonResult(items, this);
            return res;
        }

        /// <summary>
        /// Возвращает список должностей с количеством документов по фильтру
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [DimanicAuthorize("R")]
        [Route(Features.Info + "/GroupCountPositions")]
        [ResponseType(typeof(List<FrontDictionaryPosition>))]
        public IHttpActionResult PostGetGroupCountPositions([FromBody]FilterBase model)
        {
            //TODO ASYNC AWAIT
            //return await this.SafeExecuteAsync(ModelState, () =>
            //{
                var uCtx = DmsResolver.Current.Get<UserContexts>();
                var ctx = uCtx.Get();
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var items =
                    docProc.GetDocuments(ctx, model ?? new FilterBase(), new UIPaging(), EnumGroupCountType.Positions)
                        .ToList()
                        .FirstOrDefault()?
                        .DocumentWorkGroup;
                var res = new JsonResult(items, this);
                return res;
           // });
        }

        /// <summary>
        /// Возвращает документ по ИД TODO убрать вложенные элементы?
        /// </summary>
        /// <param name="Id">ИД Документа</param>
        /// <returns>Документ</returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof(FrontDocument))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, context =>
            {
                return GetById(context, Id);
            });
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
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = docProc.ExecuteAction(EnumDocumentActions.ReportRegistrationCardDocument, ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возвращает отчет Документ для подписания ЕЦП
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/ReportDocumentForDigitalSignature")]
        [ResponseType(typeof(FrontReport))]
        public IHttpActionResult GetReportDocumentForDigitalSignature(DigitalSignatureDocumentPdf model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = docProc.ExecuteAction(EnumDocumentActions.ReportDocumentForDigitalSignature, ctx, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Проверяет подписанный PDF-файл на нарушение ЕЦП
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info + "/VerifyPdf")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult VerifyPdf()
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();
            var tmpItem = false;
            try
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                using (var memoryStream = new MemoryStream())
                {
                    file.InputStream.CopyTo(memoryStream);
                    var model = memoryStream.ToArray();

                    tmpItem = (bool)encryptionProc.ExecuteAction(EnumEncryptionActions.VerifyPdf, ctx, model);
                }
            }
            catch (Exception ex)
            {
                tmpItem = false;
            }
            var res = new JsonResult(tmpItem, this);
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
            var tmpItem = Action.Execute(EnumDocumentActions.AddDocument, model, model.CurrentPositionId);
            //var res = new JsonResult(tmpItem, this);
            return GetById(context, tmpItem);
        }

        /// <summary>
        /// Корректирует документ
        /// </summary>
        /// <param name="model">Модель для обновления документа</param>
        /// <returns>Обновленный документ</returns>
        [HttpPut]
        [Route(Features.Info)]
        public IHttpActionResult Put([FromBody]ModifyDocument model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.ModifyDocument, model);
            //var res = new JsonResult(tmpItem, this);
            //res.SpentTime = stopWatch;
            return GetById(context, model.Id);
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
            Action.Execute(EnumDocumentActions.DeleteDocument, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
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
            var tmpItem = Action.Execute(EnumDocumentActions.CopyDocument, model, model.CurrentPositionId);
            //var res = new JsonResult(tmpItem, this);
            //res.SpentTime = stopWatch;
            return GetById(context, tmpItem);
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
        [HttpPut]
        public IHttpActionResult RegisterDocument([FromBody]RegisterDocument model)
        {
            var tmpItem = Action.Execute(EnumDocumentActions.RegisterDocument, model, model.CurrentPositionId);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Получает следующий регистрационный номер
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route(Features.Info + "/GetNextRegisterDocumentNumber")]
        [HttpGet]
        [ResponseType(typeof(FrontRegistrationFullNumber))]
        public IHttpActionResult GetNextRegisterDocumentNumber([FromUri]RegisterDocument model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get(model.CurrentPositionId);
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var tmpItem = docProc.GetNextRegisterDocumentNumber(ctx, model);
            var res = new JsonResult(tmpItem, this);
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
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<ICommandService>();
            var tmpItem = docProc.GetDocumentActions(ctx, Id);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Возобновляет работу с документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Info + "/StartWork")]
        [HttpPut]
        public IHttpActionResult StartWork(ChangeWorkStatus model)
        {
            Action.Execute(EnumDocumentActions.StartWork, model, model.CurrentPositionId);
            var res = new JsonResult(null, this);
            return res;
        }

        /// <summary>
        /// Заканчивает работу с документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Info + "/FinishWork")]
        [HttpPut]
        public IHttpActionResult FinishWork(ChangeWorkStatus model)
        {
            Action.Execute(EnumDocumentActions.FinishWork, model, model.CurrentPositionId);
            var res = new JsonResult(null, this);
            return res;
        }





        /// <summary>
        /// Изменяет исполнителя по документу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Info + "/ChangeExecutor")]
        [HttpPut]
        public IHttpActionResult ChangeExecutor(ChangeExecutor model)
        {
            Action.Execute(EnumDocumentActions.ChangeExecutor, model);
            var res = new JsonResult(null, this);
            return res;
        }


    }
}
