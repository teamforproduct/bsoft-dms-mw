using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;

namespace BL.Logic.AdminCore
{
    public class AdminService : IAdminService
    {
        #region AdminAccessLevels
        public BaseAdminAccessLevel GetAdminAccessLevel(IContext context, int id)
        {
            var dictDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            return dictDb.GetAdminAccessLevel(context, id);
        }

        public IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter)
        {
            var dictDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            return dictDb.GetAdminAccessLevels(context, filter);
        }
        #endregion AdminAccessLevels

    }
}
