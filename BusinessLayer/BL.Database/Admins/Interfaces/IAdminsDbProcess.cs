using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        #region AdminAccessLevels
        BaseAdminAccessLevel GetAdminAccessLevel(IContext ctx, int id);

        IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext ctx, FilterAdminAccessLevel filter);
        #endregion AdminAccessLevels

        IEnumerable<BaseAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter);
    }
}