using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        AdminAccessInfo GetAdminAccesses(IContext context);
        IEnumerable<BaseAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter);
        //bool VerifyAccess(IContext context, VerifyAccess acc, bool isThrowExeception = true);
    }
}