using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Users;
using System.Collections.Generic;

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        AdminAccessInfo GetAdminAccesses(IContext context);
        Employee GetEmployee(IContext ctx, string userId);

        IEnumerable<FrontAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter);
        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);


        #region [+] PositionRole ...
        int AddPositionRole(IContext context, InternalAdminPositionRole model);
        void UpdatePositionRole(IContext context, InternalAdminPositionRole model);
        void DeletePositionRole(IContext context, InternalAdminPositionRole model);
        bool ExistsPositionRole(IContext context, FilterAdminPositionRole filter);
        InternalAdminPositionRole GetInternalPositionRole(IContext context, FilterAdminPositionRole filter);
        IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext context, FilterAdminPositionRole filter);
        #endregion

    }
}