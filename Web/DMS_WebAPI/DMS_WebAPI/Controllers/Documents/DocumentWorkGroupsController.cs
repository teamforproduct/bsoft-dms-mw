using System.Web.Http;
using System.Web.Http.Description;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.DocumentCore.Filters;
using BL.Model.DocumentCore.FrontModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;

namespace DMS_WebAPI.Controllers.Documents
{
    [Authorize]
    public class DocumentWorkGroupsController : ApiController
    {
        /// <summary>
        /// Получение рабочей группы по документу use V3
        /// </summary>
        /// <param name="filter">модель фильтра рабочей группы</param>
        /// <param name="paging">paging</param>
        /// <returns>список подписей</returns>
        [ResponseType(typeof(List<FrontDictionaryPosition>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryPosition filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var docProc = DmsResolver.Current.Get<IDocumentService>();
            var item = docProc.GetDocumentWorkGroup(ctx, filter);
            var res = new JsonResult(item, this);
            return res;
        }
    }
}