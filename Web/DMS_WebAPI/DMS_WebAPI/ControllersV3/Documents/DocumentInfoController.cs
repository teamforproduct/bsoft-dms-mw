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
using BL.Model.EncryptionCore.InternalModel;

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
        [ResponseType(typeof (List<FrontDocument>))]
        public async Task<IHttpActionResult> PostGetList([FromBody] IncomingBase model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                if (model == null) model = new IncomingBase();
                if (model.Filter == null) model.Filter = new FilterBase();
                if (model.Paging == null) model.Paging = new UIPaging();
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var items = docProc.GetDocuments(context, model.Filter, model.Paging);
                var res = new JsonResult(items, this);
                res.Paging = model.Paging;
                return res;
            });
        }

        /// <summary>
        /// Возвращает список тегов с количеством документов по фильтру
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [DimanicAuthorize("R")]
        [Route(Features.Info + "/GroupCountTags")]
        [ResponseType(typeof (List<FrontDocumentTag>))]
        public async Task<IHttpActionResult> PostGetGroupCountTags([FromBody] FilterBase model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var items =
                    docProc.GetDocuments(context, model ?? new FilterBase(), new UIPaging {IsOnlyCounter = true},
                        EnumGroupCountType.Tags).ToList().FirstOrDefault()?.DocumentTags;
                ;
                var res = new JsonResult(items, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает список должностей с количеством документов по фильтру
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [DimanicAuthorize("R")]
        [Route(Features.Info + "/GroupCountPositions")]
        [ResponseType(typeof (List<FrontDictionaryPosition>))]
        public async Task<IHttpActionResult> PostGetGroupCountPositions([FromBody] FilterBase model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var items =
                    docProc.GetDocuments(context, model ?? new FilterBase(), new UIPaging {IsOnlyCounter = true},
                        EnumGroupCountType.Positions).ToList().FirstOrDefault()?.DocumentWorkGroup;
                var res = new JsonResult(items, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает документ по ИД TODO убрать вложенные элементы?
        /// </summary>
        /// <param name="Id">ИД Документа</param>
        /// <returns>Документ</returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}")]
        [ResponseType(typeof (FrontDocument))]
        public async Task<IHttpActionResult> Get(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
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
        [ResponseType(typeof (FrontReport))]
        public async Task<IHttpActionResult> GetReportRegistrationCardDocument(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var tmpItem = docProc.ExecuteAction(EnumDocumentActions.ReportRegistrationCardDocument, context, Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает отчет Документ для подписания ЕЦП
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/ReportDocumentForDigitalSignature")]
        [ResponseType(typeof (FrontReport))]
        public async Task<IHttpActionResult> GetReportDocumentForDigitalSignature(DigitalSignatureDocumentPdf model)
        {
            model.ServerPath = Properties.Settings.Default.ServerPath;
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var tmpItem = docProc.ExecuteAction(EnumDocumentActions.ReportDocumentForDigitalSignature, context,model);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Проверяет подписанный PDF-файл на нарушение ЕЦП
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info + "/VerifyPdf")]
        [ResponseType(typeof (bool))]
        public async Task<IHttpActionResult> VerifyPdf()
        {
            var model = new VerifyPdfCertificate();
            using (var memoryStream = new MemoryStream())
            {
                var file = HttpContext.Current.Request.Files[0];
                file.InputStream.CopyTo(memoryStream);
                model.ServerPath = Properties.Settings.Default.ServerPath;
                model.FileData = memoryStream.ToArray();
            }
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var md = (VerifyPdfCertificate) param;
                var encryptionProc = DmsResolver.Current.Get<IEncryptionService>();
                bool tmpItem;
                try
                {
                    tmpItem = (bool) encryptionProc.ExecuteAction(EnumEncryptionActions.VerifyPdf, context, md);
                }
                catch (Exception)
                {
                    tmpItem = false;
                }
                var res = new JsonResult(tmpItem, this);
                return res;
            }, model);
        }

        /// <summary>
        /// Добавляет документ по шаблону
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Post([FromBody] AddDocumentByTemplateDocument model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDocumentActions.AddDocument, model, model.CurrentPositionId);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Корректирует документ
        /// </summary>
        /// <param name="model">Модель для обновления документа</param>
        /// <returns>Обновленный документ</returns>
        [HttpPut]
        [Route(Features.Info)]
        public async Task<IHttpActionResult> Put([FromBody] ModifyDocument model)
        {
            model.ServerPath = Properties.Settings.Default.ServerPath;
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.ModifyDocument, model);
                return GetById(context, model.Id);
            });
        }

        /// <summary>
        /// Удаляет документ
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Info + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete(int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.DeleteDocument, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Копирует документ 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Добавленный документ</returns>
        [Route(Features.Info + "/Duplicate")]
        [HttpPost]
        public async Task<IHttpActionResult> CopyDocument([FromBody] CopyDocument model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDocumentActions.CopyDocument, model, model.CurrentPositionId);
                return GetById(context, tmpItem);
            });
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
        public async Task<IHttpActionResult> RegisterDocument([FromBody] RegisterDocument model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDocumentActions.RegisterDocument, model,
                    model.CurrentPositionId);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Получает следующий регистрационный номер
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Обновленный документ</returns>
        [Route(Features.Info + "/GetNextRegisterDocumentNumber")]
        [HttpGet]
        [ResponseType(typeof (FrontRegistrationFullNumber))]
        public async Task<IHttpActionResult> GetNextRegisterDocumentNumber([FromUri] RegisterDocument model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentService>();
                var tmpItem = docProc.GetNextRegisterDocumentNumber(context, model);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает меню для работы с документом 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Info + "/{Id:int}" + "/Actions")]
        [ResponseType(typeof (List<InternalDictionaryPositionWithActions>))]
        public async Task<IHttpActionResult> Actions([FromUri] int Id)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<ICommandService>();
                var tmpItem = docProc.GetDocumentActions(context, Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

        /// <summary>
        /// Возобновляет работу с документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Info + "/StartWork")]
        [HttpPut]
        public async Task<IHttpActionResult> StartWork(ChangeWorkStatus model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.StartWork, model, model.CurrentPositionId);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Заканчивает работу с документом
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Info + "/FinishWork")]
        [HttpPut]
        public async Task<IHttpActionResult> FinishWork(ChangeWorkStatus model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.FinishWork, model, model.CurrentPositionId);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Изменяет исполнителя по документу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route(Features.Info + "/ChangeExecutor")]
        [HttpPut]
        public async Task<IHttpActionResult> ChangeExecutor(ChangeExecutor model)
        {
            return await this.SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.ChangeExecutor, model);
                var res = new JsonResult(null, this);
                return res;
            });
        }

    }
}
