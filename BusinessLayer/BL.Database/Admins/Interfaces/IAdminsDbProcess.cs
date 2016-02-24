using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Enums;

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        #region AdminAccessLevels
        BaseAdminAccessLevel GetAdminAccessLevel(IContext ctx, int id);

        IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext ctx, FilterAdminAccessLevel filter);
        #endregion AdminAccessLevels

        IEnumerable<BaseAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter);
        bool VerifyAccess(IContext context, EnumDocumentActions action, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, VerifyAccess acc, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, string actionCode, bool isThrowExeception = true);
    }
}