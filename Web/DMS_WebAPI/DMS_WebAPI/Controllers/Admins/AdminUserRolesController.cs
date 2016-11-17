using BL.Logic.AdminCore.Interfaces;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using System.Collections.Generic;
using System.Web.Http.Description;
using System.Diagnostics;

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Описывает список ролей, которые доступны конкретному сотруднику-пользователю.
    /// При назначении сотрудника да должность роли, назначенные должности, передаются сотруднику (роли могут передаваться не в полном объеме - референт может не иметь права подписания)
    /// </summary>
    [Authorize]
    public class AdminUserRolesController : ApiController
    {
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Возвращает список ролей, которые доступны конкретному сотруднику-пользователю.
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [ResponseType(typeof(List<FrontAdminUserRole>))]
        public IHttpActionResult Get([FromUri] int userId, [FromUri] FilterDIPAdminUserRole filter)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetUserRolesDIP(ctx, userId, filter);
            var res = new JsonResult(tmpItems, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// GetAdminUserRoles by ID 
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminUserRole</returns>
        [ResponseType(typeof(FrontAdminUserRole))]
        public IHttpActionResult Get(int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetUserRoles(ctx, new FilterAdminUserRole() { IDs = new List<int> { id } });
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }

        /// <summary>
        /// Добавляет роль сотруднику-пользователю
        /// </summary>
        /// <param name="model">ModifyAdminUserRole</param>
        /// <returns>FrontAdminUserRole</returns>
        public IHttpActionResult Post([FromBody]ModifyAdminUserRole model)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddUserRole, cxt, model);
            return Get((int)tmpItem);
        }

        /// <summary>
        /// Отнимает роль у сотрудника-пользователя
        /// </summary>
        /// <returns>FrontAdminUserRole</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            if (!stopWatch.IsRunning) stopWatch.Restart();
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();

            tmpService.ExecuteAction(EnumAdminActions.DeleteUserRole, cxt, id);
            var tmpItem = new FrontAdminUserRole() { Id = id };
            var res = new JsonResult(tmpItem, this);
            res.SpentTime = stopWatch;
            return res;
        }
    }
}