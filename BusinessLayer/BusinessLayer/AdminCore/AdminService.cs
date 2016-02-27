using System.Collections.Generic;
using BL.Logic.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using System;
using BL.CrossCutting.Interfaces;
using BL.Model.Enums;
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
        /// <param name="model"></param>
        /// <param name="isThrowExeception"></param>
        /// <param name="context"></param>
        public bool VerifyAccess(IContext context, VerifyAccess model, bool isThrowExeception = true)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            return admDb.VerifyAccess(context, model, isThrowExeception);
        }

        public bool VerifyAccess(IContext context, EnumDocumentActions action, bool isThrowExeception = true)
        {
            return VerifyAccess(context, new VerifyAccess { DocumentActionCode = action.ToString() }, isThrowExeception);
        }
    }
}
