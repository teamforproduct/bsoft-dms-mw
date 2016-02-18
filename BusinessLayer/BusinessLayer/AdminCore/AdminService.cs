using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using System;
using BL.Model.Exception;

namespace BL.Logic.AdminCore
{
    public class AdminService : IAdminService
    {
        #region AdminAccessLevels
        public BaseAdminAccessLevel GetAdminAccessLevel(IContext context, int id)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            return admDb.GetAdminAccessLevel(context, id);
        }

        public IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            return admDb.GetAdminAccessLevels(context, filter);
        }

        #endregion AdminAccessLevels

        public IEnumerable<BaseAdminUserRole> GetPositionsByCurrentUser(IContext context)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            return admDb.GetPositionsByUser(context, new FilterAdminUserRole() { UserId  = new List<int>() { context.CurrentAgentId } });
        }

        /// <summary>
        /// Проверка доступа к должностям для текущего пользователя
        /// </summary>
        /// <param name="cxt"></param>
        /// <param name="positionsIdList"></param>
        public void VerifyAccess(IContext context, VerifyAccess model)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            admDb.VerifyAccess(context, model);
        }
    }
}
