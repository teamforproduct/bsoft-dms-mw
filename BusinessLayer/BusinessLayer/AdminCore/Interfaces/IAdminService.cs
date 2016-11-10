using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.Enums;
using BL.Model.Users;
using System.Collections.Generic;
using BL.Model.Tree;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface IAdminService
    {
        #region [+] General ...

        object ExecuteAction(EnumAdminActions act, IContext context, object param);

        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);
        IEnumerable<FrontAvailablePositions> GetAvailablePositions(IContext context);
        Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext context);

        Employee GetUserForContext(IContext context, string userId);

        #endregion

        #region [+] Verify ...
        bool VerifyAccess(IContext context, VerifyAccess verifyAccess, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumDocumentActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumDictionaryActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumAdminActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumEncryptionActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifyAccess(IContext context, EnumSystemActions action, bool isPositionFromContext = true, bool isThrowExeception = true);
        bool VerifySubordination(IContext context, VerifySubordination model);
        #endregion

        #region [+] Roles ...
        //FrontAdminRole GetAdminRole(IContext context, int id);
        int AddNamedRole(IContext context, string code, string name, IEnumerable<InternalAdminRoleAction> roleActions);
        IEnumerable<FrontAdminRole> GetAdminRoles(IContext context, FilterAdminRole filter);
        #endregion

        #region [+] RoleActions ...
        IEnumerable<FrontAdminRoleAction> GetRoleActions(IContext context, FilterAdminRoleAction filter);
        #endregion

        #region [+] PositionRoles ...
        IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext context, FilterAdminRole filter);

        FrontAdminPositionRole GetPositionRole(IContext context, int id);
        #endregion

        #region [+] UserRoles ...
        IEnumerable<FrontAdminUserRole> GetUserRoles(IContext context, FilterAdminUserRole filter);
        //IEnumerable<FrontAdminUserRole> GetUserRolesDIP(IContext context, FilterAdminRole filter);
        IEnumerable<ITreeItem> GetUserRolesDIP(IContext context, int userId, FilterDIPAdminUserRole filter);
        void AddAllPositionRoleForUser(IContext context, ModifyDictionaryPositionExecutor positionExecutor);
        #endregion

        #region [+] DepartmentAdmins ...
        IEnumerable<FrontDictionaryAgentEmployee> GetDepartmentAdmins(IContext context, int departmentId);
        #endregion

        #region [+] Subordinations ...
        IEnumerable<FrontAdminSubordination> GetAdminSubordinations(IContext context, FilterAdminSubordination filter);

        IEnumerable<ITreeItem> GetSubordinationsDIP(IContext context, int positionId, FilterAdminSubordinationTree filter);
        #endregion

        #region [+] RegistrationJournalPositions ...
        IEnumerable<ITreeItem> GetRegistrationJournalPositionsDIP(IContext context, int positionId, FilterTree filter);
        #endregion
    }
}
