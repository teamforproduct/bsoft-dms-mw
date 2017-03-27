using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Рабочая группа.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentWorkGroupController : ApiController
    {
        /// <summary>
        /// Возвращает список рабочей группы
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="paging">Пейджинг</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.WorkGroups)]
        [ResponseType(typeof(List<FrontDictionaryPosition>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryPosition filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocumentWorkGroup(ctx, filter, paging);
            var res = new JsonResult(items, this);
            return res;
        }

        /// <summary>
        /// Возвращает список доступов для документов
        /// </summary>
        /// <param name="filter">Фильтр</param>
        /// <param name="paging">Пейджинг</param>
        /// <returns></returns>
        [HttpGet]
        [Route(Features.WorkGroups+ "/Access")]
        [ResponseType(typeof(List<FrontDocumentAccess>))]
        public IHttpActionResult Get([FromUri] FilterDocumentAccess filter, [FromUri]UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var items = docProc.GetDocumentAccesses(ctx, filter, paging);
            var res = new JsonResult(items, this);
            return res;
        }

    }
}
