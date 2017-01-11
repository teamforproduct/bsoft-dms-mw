using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.SystemCore;
using BL.CrossCutting.DependencyInjection;
using System.Web.Http.Description;
using System.Collections.Generic;

using BL.Model.Common;
using System.Web;
using BL.Logic.SystemServices.TempStorage;
using BL.Model.DictionaryCore.FrontMainModel;
using System.Diagnostics;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Tree;

namespace DMS_WebAPI.ControllersV3.Employees
{
    /// <summary>
    /// Роли сотрудников. Регулирует роли (обязанности) сотрудников. Сотрудник может выполнять не все обязанности должности на которую он назначен, временно исполняет или реферирует.
    /// Например, референт директора может не иметь права подписания и т.д. 
    /// </summary>
    [Authorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Employee)]
    public class EmployeeRolesController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает роли пользователя с пычками
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Roles)]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult Get([FromUri] int Id, [FromUri] FilterDIPAdminUserRole filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterDIPAdminUserRole();
            filter.IsChecked = true;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetUserRolesDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Режим корректировки. Возвращает полный список ролей (который предусмотрен для должности) с пычками
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Roles + "/Edit")]
        [ResponseType(typeof(List<TreeItem>))]
        public IHttpActionResult GetEdit([FromUri] int Id, [FromUri] FilterDIPAdminUserRole filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            if (filter == null) filter = new FilterDIPAdminUserRole();
            filter.IsChecked = false;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetUserRolesDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает роли сотруднику
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Roles + "/Set")]
        public IHttpActionResult Set([FromBody] SetUserRole model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetUserRole, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Устанавливает все роли должности сотруднику
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Roles + "/SetByAssignment")]
        public IHttpActionResult SetByDepartment([FromBody] SetUserRoleByAssignment model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var tmpItem = Action.Execute(EnumAdminActions.SetUserRoleByAssignment, model);
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }


    }
}
