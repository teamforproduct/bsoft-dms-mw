using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Common;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
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
    /// Документы. Ограничительный список рассылки.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentAccessListController : WebApiController
    {
        private IHttpActionResult GetById(IContext ctx, int Id)
        {
            var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
            var item = docProc.GetRestrictedSendList(ctx, Id);
            var res = new JsonResult(item, this);
            return res;
        }

        /// <summary>
        /// Возвращает ограничительный список по ИД документа
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.AccessList)]
        [ResponseType(typeof(List<FrontDocumentRestrictedSendList>))]
        public async Task<IHttpActionResult> GetByDocumentId(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
                var items = docProc.GetRestrictedSendLists(context, Id);
                var res = new JsonResult(items, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает ограничительный список по ИД документа для автокомплита
        /// </summary>
        /// <param name="Id">ИД документа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.AccessList + "/ListForAutocomplete")]
        [ResponseType(typeof(List<AutocompleteItem>))]
        public async Task<IHttpActionResult> GetByDocumentIdListForAutocomplete(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
                var items = docProc.GetRestrictedSendListsForAutocomplete(context, Id);
                var res = new JsonResult(items, this);
                return res;
            });
        }

        /// <summary>
        /// Возвращает запись ограничительного списка по ИД
        /// </summary>
        /// <param name="Id">ИД ограничительного списка</param>
        /// <returns>запись ограничительного списка</returns>
        [HttpGet]
        [Route(Features.AccessList + "/{Id:int}")]
        [ResponseType(typeof(FrontDocumentRestrictedSendList))]
        public async Task<IHttpActionResult> GetById(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var docProc = DmsResolver.Current.Get<IDocumentSendListService>();
                var item = docProc.GetRestrictedSendList(context, Id);
                var res = new JsonResult(item, this);
                return res;
            });
        }

        /// <summary>
        /// Добавляет запись ограничительного списка
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.AccessList)]
        public async Task<IHttpActionResult> Post([FromBody]ModifyDocumentRestrictedSendList model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                var tmpItem = Action.Execute(context, EnumDocumentActions.AddDocumentRestrictedSendList, model);
                return GetById(context, tmpItem);
            });
        }

        /// <summary>
        /// Добавляет записи ограничительного списка по стандартному списку
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Features.AccessList + "/ByStandartSendList")]
        public async Task<IHttpActionResult> Post([FromBody]ModifyDocumentRestrictedSendListByStandartSendList model)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.AddByStandartSendListDocumentRestrictedSendList, model);
                var res = new JsonResult(null, this);
                return res;
            });
        }

        /// <summary>
        /// Удаляет запись ограничительного списка
        /// </summary>
        /// <param name="Id">ИД записи ограничительного списка</param>
        /// <returns></returns>
        [HttpDelete]
        [Route(Features.AccessList + "/{Id:int}")]
        public async Task<IHttpActionResult> Delete(int Id)
        {
            return await SafeExecuteAsync(ModelState, (context, param) =>
            {
                Action.Execute(context, EnumDocumentActions.DeleteDocumentRestrictedSendList, Id);
                var tmpItem = new FrontDeleteModel(Id);
                var res = new JsonResult(tmpItem, this);
                return res;
            });
        }

    }
}
