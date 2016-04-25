using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Users;

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        AdminAccessInfo GetAdminAccesses(IContext context);
        IEnumerable<FrontAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter);

        Employee GetEmployee(IContext ctx, string userId);
        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);
    }
}