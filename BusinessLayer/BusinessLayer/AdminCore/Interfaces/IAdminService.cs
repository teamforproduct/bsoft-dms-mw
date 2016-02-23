using BL.Model.AdminCore;
using System.Collections.Generic;
using BL.CrossCutting.Interfaces;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface IAdminService
    {
        #region AdminAccessLevels
        BaseAdminAccessLevel GetAdminAccessLevel(IContext context, int id);

        IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter);
        #endregion AdminAccessLevels
        IEnumerable<BaseAdminUserRole> GetPositionsByCurrentUser(IContext context);
        void VerifyAccess(IContext context, VerifyAccess verifyAccess);
    }
}
