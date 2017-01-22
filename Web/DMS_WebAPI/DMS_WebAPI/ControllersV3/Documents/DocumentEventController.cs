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
    /// Документы. События.
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Document)]
    public class DocumentEventController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список событий
        /// </summary>
        /// <param name="model">Входящая модель</param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.Events)]
        [ResponseType(typeof(List<FrontDocumentEvent>))]
        public IHttpActionResult PostGetList([FromBody]IncomingBase model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (model == null) model = new IncomingBase();
            if (model.Filter == null) model.Filter = new FilterBase();
            if (model.Paging == null) model.Paging = new UIPaging();

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocumentEvents(ctx, model.Filter, model.Paging);
            var res = new JsonResult(items, this);
            res.Paging = model.Paging;
            res.SpentTime = stopWatch;
            return res;
        }        

        /// <summary>
        /// Возвращает событие по ИД
        /// </summary>
        /// <param name="Id">ИД события</param>
        /// <returns>Событие</returns>
        [HttpGet]
        [Route(Features.Events + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentEvent))]
        public IHttpActionResult Get(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocumentEvent(ctx, Id);
            var res = new JsonResult(item, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает меню для работы с документом по событию
        /// </summary>
        /// <param name="Id">ИД события</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.Events + "/{Id:int}" + "/Actions")]
        [ResponseType(typeof(List<InternalDictionaryPositionWithActions>))]
        public IHttpActionResult Actions([FromUri]int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<ICommandService>();
            var items = docProc.GetDocumentActions(ctx, Id, Id);
            var res = new JsonResult(items, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Отмечает для пользователя ивенты как прочтенные
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Events + "/MarkDocumentEventAsRead")]
        public IHttpActionResult MarkDocumentEventAsRead([FromBody]MarkDocumentEventAsRead model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.MarkDocumentEventAsRead, model);
            var res = new JsonResult(true, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
