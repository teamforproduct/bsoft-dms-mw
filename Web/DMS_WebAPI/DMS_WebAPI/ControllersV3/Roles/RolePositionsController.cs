using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;
using BL.Model.Common;
using System.Diagnostics;
using BL.Model.SystemCore;

namespace DMS_WebAPI.ControllersV3.Roles
{
    /// <summary>
    /// Роли. Должности, которым назначена роль
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Role)]
    public class RolePositionsController : ApiController
    {
        /// <summary>
        /// Возвращает список должностей
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Positions)]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterDictionaryPosition filter, UIPaging paging)
        {
            if (filter == null) filter = new FilterDictionaryPosition();
            filter.RoleIDs = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetPositionList(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            return res;
        }
    }
}