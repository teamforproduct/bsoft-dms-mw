using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Users;
using System.Collections.Generic;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface IAdminService
    {
        #region [+] General ...

        object ExecuteAction(EnumAdminActions act, IContext context, object param);

        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);
        IEnumerable<FrontAdminUserRole> GetPositionsByCurrentUser(IContext context);
        Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext context);

        Employee GetEmployee(IContext context, string userId);

        #endregion

        #region [+] Verify ...
        bool VerifyAccess(IContext context, VerifyAccess verifyAccess, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumDocumentActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumDictionaryActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumAdminActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumEncryptionActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifySubordination(IContext context, VerifySubordination model);
        #endregion

        #region [+] Roles ...
        //FrontAdminRole GetAdminRole(IContext context, int id);

        IEnumerable<FrontAdminRole> GetAdminRoles(IContext context, FilterAdminRole filter);
        #endregion

        #region [+] RoleActions ...
        IEnumerable<FrontAdminRoleAction> GetAdminRoleActions(IContext context, FilterAdminRoleAction filter);
        #endregion

        #region [+] PositionRoles ...
        IEnumerable<FrontAdminPositionRole> GetAdminPositionRoles(IContext context, FilterAdminPositionRole filter);
        #endregion

        #region [+] UserRoles ...
        IEnumerable<FrontAdminUserRole> GetAdminUserRoles(IContext context, FilterAdminUserRole filter);
        #endregion

        #region [+] Subordinations ...
        IEnumerable<FrontAdminSubordination> GetAdminSubordinations(IContext context, FilterAdminSubordination filter);
        #endregion

    }
}
