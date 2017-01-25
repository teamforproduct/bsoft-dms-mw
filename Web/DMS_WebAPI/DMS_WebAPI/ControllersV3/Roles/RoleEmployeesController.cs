using BL.CrossCutting.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Roles
{
    /// <summary>
    /// Сотрудники, которым назначена роль
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Role)]
    public class RoleEmployeesController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список сотрудников
        /// </summary>
        /// <param name="Id">ИД сотрудника</param>
        /// <param name="filter">параметры фильтрации</param>
        /// <param name="paging">параметры фильтрации</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Employees)]
        [ResponseType(typeof(List<ListItem>))]
        public IHttpActionResult Get(int Id, [FromUri] FilterDictionaryAgentEmployee filter, [FromUri]UIPaging paging)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterDictionaryAgentEmployee();
            filter.RoleIDs = new List<int> { Id };

            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IDictionaryService>();
            var tmpItems = tmpService.GetAgentEmployeeList(ctx, filter, paging);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}