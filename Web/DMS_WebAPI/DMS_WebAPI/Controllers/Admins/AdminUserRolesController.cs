﻿using BL.Logic.AdminCore.Interfaces;
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

namespace DMS_WebAPI.Controllers.Admins
{
    /// <summary>
    /// Описывает список ролей, которые доступны конкретному сотруднику-пользователю.
    /// При назначении сотрудника да должность роли, назначенные должности, передаются сотруднику (роли могут передаваться не в полном объеме - референт может не иметь права подписания)
    /// </summary>
    [Authorize]
    public class AdminUserRolesController : ApiController
    {
        /// <summary>
        /// Возвращает список ролей, которые доступны конкретному сотруднику-пользователю.
        /// </summary>
        /// <param name="filter">Filter parms</param>
        /// <returns>FrontAdminPositions</returns>
        [ResponseType(typeof(List<FrontAdminUserRole>))]
        public IHttpActionResult Get([FromUri] int userId, [FromUri] FilterAdminRole filter)
        {
            if (filter.UserIDs == null) filter.UserIDs = new List<int>();

            filter.UserIDs.Add(userId);

            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItems = tmpService.GetAdminUserRolesDIP(ctx, filter);
            return new JsonResult(tmpItems, this);
        }

        /// <summary>
        /// GetAdminUserRoles by ID 
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>FrontAdminUserRole</returns>
        [ResponseType(typeof(FrontAdminUserRole))]
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.GetAdminUserRoles(ctx, new FilterAdminUserRole() { IDs = new List<int> { id } });
            return new JsonResult(tmpItem, this);
        }

        /// <summary>
        /// Добавляет роль сотруднику-пользователю
        /// </summary>
        /// <param name="model">ModifyAdminUserRole</param>
        /// <returns>FrontAdminUserRole</returns>
        public IHttpActionResult Post([FromBody]ModifyAdminUserRole model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();
            var tmpItem = tmpService.ExecuteAction(EnumAdminActions.AddUserRole, cxt, model);
            return Get((int)tmpItem);
        }

        ///// <summary>
        ///// Изменяет роль сотруднику-пользователю. Например, период исполнения роли
        ///// </summary>
        ///// <param name="id">Record Id</param>
        ///// <param name="model">ModifyAdminUserRole</param>
        ///// <returns>FrontAdminUserRole</returns>
        //public IHttpActionResult Put(int id, [FromBody]ModifyAdminUserRole model)
        //{
        //    model.Id = id;
        //    var cxt = DmsResolver.Current.Get<UserContext>().Get();
        //    var tmpService = DmsResolver.Current.Get<IAdminService>();
        //    tmpService.ExecuteAction(EnumAdminActions.ModifyUserRole, cxt, model);
        //    return Get(model.Id);
        //}

        /// <summary>
        /// Отнимает роль у сотрудника-пользователя
        /// </summary>
        /// <returns>FrontAdminUserRole</returns> 
        public IHttpActionResult Delete([FromUri] int userId, [FromUri] int roleId)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpService = DmsResolver.Current.Get<IAdminService>();

            tmpService.ExecuteAction(EnumAdminActions.DeleteUserRole, cxt, new ModifyAdminUserRole() { UserId = userId, RoleId = roleId });
            FrontAdminUserRole tmpItem = new FrontAdminUserRole() { UserId = userId, RoleId = roleId };
            return new JsonResult(tmpItem, this);
        }
    }
}