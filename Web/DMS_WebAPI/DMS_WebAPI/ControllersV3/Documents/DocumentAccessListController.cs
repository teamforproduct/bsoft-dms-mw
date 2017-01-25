using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
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
    /// Документы. Ограничительный список рассылки.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentAccessListController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает ограничительный список по ИД документа
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/"+Features.AccessList)]
        [ResponseType(typeof(List<FrontDocumentRestrictedSendList>))]
        public IHttpActionResult GetByDocumentId(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            var items = docProc.GetRestrictedSendLists(ctx, Id);
            var res = new JsonResult(items, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Возвращает запись ограничительного списка по ИД
        /// </summary>
        /// <param name="Id">ИД ограничительного списка</param>
        /// <returns>запись ограничительного списка</returns>
        [HttpGet]
        [Route(Features.AccessList + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentRestrictedSendList))]
        public IHttpActionResult GetById(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            var item = docProc.GetRestrictedSendList(ctx, Id);
            var res = new JsonResult(item, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет запись ограничительного списка
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.AccessList)]
        public IHttpActionResult Post([FromBody]ModifyDocumentRestrictedSendList model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumDocumentActions.AddDocumentRestrictedSendList, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет записи ограничительного списка по стандартному списку
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.AccessList + "/ByStandartSendList")]
        public IHttpActionResult Post([FromBody]ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.AddByStandartSendListDocumentRestrictedSendList, model);
            var res = new JsonResult(null, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Удаляет запись ограничительного списка
        /// </summary>
        /// <param name="Id">ИД записи ограничительного списка</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.AccessList + "/{Id:int}")]
        public IHttpActionResult Delete(int Id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            Action.Execute(EnumDocumentActions.DeleteDocumentRestrictedSendList, Id);
            var tmpItem = new FrontDeleteModel(Id);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

    }
}
