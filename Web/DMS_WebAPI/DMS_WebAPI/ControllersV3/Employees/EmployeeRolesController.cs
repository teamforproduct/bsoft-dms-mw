using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.SystemCore;
using BL.Model.Tree;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.ControllersV3.Employees
{
    /// <summary>
    /// Сотрудники. Роли. Регулирует роли (обязанности) сотрудников. Сотрудник может выполнять не все обязанности должности на которую он назначен, временно исполняет или реферирует.
    /// Например, референт директора может не иметь права подписания и т.д. 
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Employee)]
    public class EmployeeRolesController : ApiController
    {
        /// <summary>
        /// Возвращает роли пользователя с пычками
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Roles)]
        [ResponseType(typeof(List<TreeItem>))]
        public async Task<IHttpActionResult> Get([FromUri] int Id, [FromUri] FilterDIPAdminUserRole filter)
        {
            if (filter == null) filter = new FilterDIPAdminUserRole();
            filter.IsChecked = true;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetUserRolesDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Возвращает роли пользователя с пычками
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Roles+ "/Current")]
        [ResponseType(typeof(List<TreeItem>))]
        public async Task<IHttpActionResult> GetCurrent([FromUri] int Id, [FromUri] FilterDIPAdminUserRole filter)
        {
            if (filter == null) filter = new FilterDIPAdminUserRole();
            filter.IsChecked = true;
            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetUserRolesDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
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
        public async Task<IHttpActionResult> GetEdit([FromUri] int Id, [FromUri] FilterDIPAdminUserRole filter)
        {
            if (filter == null) filter = new FilterDIPAdminUserRole();
            filter.IsChecked = null;
            //filter.StartDate = DateTime.UtcNow;
            //filter.EndDate = DateTime.UtcNow;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetUserRolesDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Режим корректировки. Возвращает полный список ролей (который предусмотрен для должности) с пычками
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="filter">"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Id:int}/" + Features.Roles + "/Current/Edit")]
        [ResponseType(typeof(List<TreeItem>))]
        public async Task<IHttpActionResult> GetCurrentEdit([FromUri] int Id, [FromUri] FilterDIPAdminUserRole filter)
        {
            if (filter == null) filter = new FilterDIPAdminUserRole();
            filter.IsChecked = null;
            filter.StartDate = DateTime.UtcNow;
            filter.EndDate = DateTime.UtcNow;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetUserRolesDIP(ctx, Id, filter);
            var res = new JsonResult(tmpItems, this);
            return res;
        }

        /// <summary>
        /// Устанавливает роли сотруднику
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Roles + "/Set")]
        public async Task<IHttpActionResult> Set([FromBody] SetUserRole model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.SetUserRole, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }

        /// <summary>
        /// Устанавливает все роли должности сотруднику
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route(Features.Roles + "/SetByAssignment")]
        public async Task<IHttpActionResult> SetByDepartment([FromBody] ItemCheck model)
        {
            var tmpItem = Action.Execute(EnumAdminActions.SetUserRoleByAssignment, model);
            var res = new JsonResult(tmpItem, this);
            return res;
        }


    }
}
