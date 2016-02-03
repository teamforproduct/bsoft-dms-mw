using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        #region AdminAccessLevels
        BaseAdminAccessLevel GetAdminAccessLevel(IContext context, int id);

        IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter);
        #endregion AdminAccessLevels
    }
}