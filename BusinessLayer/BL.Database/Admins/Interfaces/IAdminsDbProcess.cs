using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Users;

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        AdminAccessInfo GetAdminAccesses(IContext context);
        IEnumerable<BaseAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter);
        //bool VerifyAccess(IContext context, VerifyAccess acc, bool isThrowExeception = true);

        Employee GetEmployee(IContext ctx, int id);
        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);
    }
}