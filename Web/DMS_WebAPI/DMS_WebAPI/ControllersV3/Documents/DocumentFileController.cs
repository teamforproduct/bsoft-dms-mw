using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Файлы.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentFileController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список файлов
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [DimanicAuthorize("R")]
        [Route(Features.Files + "/Main")]
        [ResponseType(typeof(List<FrontDocumentAttachedFile>))]
        public IHttpActionResult PostGetList([FromBody]IncomingBase model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (model == null) model = new IncomingBase();
            if (model.Filter == null) model.Filter = new FilterBase();
            if (model.Paging == null) model.Paging = new UIPaging();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFileService>();
            var items = docProc.GetDocumentFiles(ctx, model.Filter, model.Paging);
            var res = new JsonResult(items, this);
            res.Paging = model.Paging;
            res.SpentTime = stopWatch;
            return res;
        }        

        /// <summary>
        /// Возвращает файл по ИД TODO переделать параметр
        /// </summary>
        /// <param name="Id">ИД события</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route(Features.Files + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentAttachedFile))]
        public IHttpActionResult Get([FromUri]FilterDocumentFileIdentity Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentFileService>();
            var item = docProc.GetUserFile(ctx, Id);
            var res = new JsonResult(item, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет файл, если файл с таким именем есть создается новая версия файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Files)]
        public IHttpActionResult Post([FromUri]AddDocumentFile model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;
            model.IsUseMainNameFile = false;
            var tmpItem = Action.Execute(EnumDocumentActions.AddDocumentFile, model, model.CurrentPositionId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет версию файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Files)]
        public IHttpActionResult PostAddUseMainNameFile([FromUri]AddDocumentFile model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            model.PostedFileData = file;
            model.FileName = file.FileName;
            model.FileType = file.ContentType;
            model.IsUseMainNameFile = true;
            var tmpItem = Action.Execute(EnumDocumentActions.AddDocumentFileUseMainNameFile, model, model.CurrentPositionId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Измененяет описание файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/ChangeDescription")]
        public IHttpActionResult Put([FromBody]ModifyDocumentFile model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.ModifyDocumentFile, model,model.CurrentPositionId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Измененяет  имя файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/Rename")]
        public IHttpActionResult RenameFile([FromBody]ModifyDocumentFile model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.RenameDocumentFile, model, model.CurrentPositionId);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }


        /// <summary>
        /// Принимает версию файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/AcceptVersion")]
        public IHttpActionResult AcceptVersion([FromBody]ChangeWorkOutDocumentFile model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.AcceptDocumentFile, model);
            var res = new JsonResult(true, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Делает версию файла основной
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/AcceptMainVersion")]
        public IHttpActionResult AcceptMainVersion([FromBody]ChangeWorkOutDocumentFile model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.AcceptMainVersionDocumentFile, model);
            var res = new JsonResult(true, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Отклоняет версию файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Files + "/RejectVersion")]
        public IHttpActionResult RejectVersion([FromBody]ChangeWorkOutDocumentFile model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.RejectDocumentFile, model);
            var res = new JsonResult(true, this);
            res.SpentTime = stopWatch;
            return res;
        }



        /// <summary>
        /// Удаляет файла (удадяет все версии) (ставится отметка или полное удаление)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Files)]
        public IHttpActionResult Delete([FromUri]FilterDocumentFileIdentity model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteDocumentFile, model);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Удаляет версию файла (ставится отметка)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Files+ "/DeleteFileVersion")]
        public IHttpActionResult DeleteFileVersion([FromUri]FilterDocumentFileIdentity model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteDocumentFileVersion, model);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Удаляет версию файла (окончательное удаление)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.Files + "/DeleteFileVersionRecord")]
        public IHttpActionResult DeleteFileVersionRecord([FromUri]FilterDocumentFileIdentity model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteDocumentFileVersionRecord, model);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает меню по ИД документа для работы с файлами 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Files + "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public IHttpActionResult ActionsByDocument([FromUri]int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<ICommandService>();
            var items = docProc.GetDocumentFileActions(ctx, Id);
            var res = new JsonResult(items, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает меню по ИД файла для работы с файлами 
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Files+"/{Id:int}" + "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public IHttpActionResult ActionsByFile([FromUri]int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<ICommandService>();
            var items = docProc.GetDocumentFileActions(ctx, null, Id);
            var res = new JsonResult(items, this);
            res.SpentTime = stopWatch;
            return res;
        }


    }
}
