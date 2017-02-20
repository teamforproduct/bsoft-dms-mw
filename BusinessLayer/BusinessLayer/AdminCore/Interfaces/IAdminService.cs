using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.FullTextSearch;
using BL.Model.SystemCore;
using BL.Model.SystemCore.FrontModel;
using BL.Model.Tree;
using BL.Model.Users;
using System.Collections.Generic;

namespace BL.Logic.AdminCore.Interfaces
{
    public interface IAdminService
    {
        #region [+] General ...

        object ExecuteAction(EnumAdminActions act, IContext context, object param);

        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);
        IEnumerable<FrontUserAssignments> GetAvailablePositions(IContext context, List<int> PositionIDs = null);
        Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext context);

        Employee GetEmployeeForContext(IContext context, string userId);
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
        int AddNamedRole(IContext context, string code, string name, IEnumerable<InternalAdminRolePermission> roleActions);
        FrontAdminRole GetAdminRole(IContext context, int id);
        IEnumerable<ListItem> GetListRoles(IContext context, FilterAdminRole filter, UIPaging paging);
        IEnumerable<FrontAdminRole> GetAdminRoles(IContext context, FilterAdminRole filter);
        IEnumerable<ListItem> GetMainRoles(IContext context, FullTextSearch ftSearch, FilterAdminRole filter, UIPaging paging);
        #endregion


        #region [+] PositionRoles ...
        IEnumerable<FrontAdminPositionRole> GetPositionRolesDIP(IContext context, FilterAdminPositionRoleDIP filter);
        IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext context, FilterAdminPositionRole filter);
        FrontAdminPositionRole GetPositionRole(IContext context, int id);
        #endregion

        #region [+] UserRoles ...
        IEnumerable<FrontAdminUserRole> GetUserRoles(IContext context, FilterAdminUserRole filter);
        //IEnumerable<FrontAdminUserRole> GetUserRolesDIP(IContext context, FilterAdminRole filter);
        IEnumerable<ITreeItem> GetUserRolesDIP(IContext context, int userId, FilterDIPAdminUserRole filter);
        void AddAllPositionRoleForUser(IContext context, InternalDictionaryPositionExecutor positionExecutor);
        #endregion

        #region [+] DepartmentAdmins ...
        IEnumerable<FrontAdminEmployeeDepartments> GetDepartmentAdmins(IContext context, int departmentId);
        #endregion

        #region [+] Subordinations ...

        IEnumerable<ITreeItem> GetSubordinationsDIP(IContext context, int positionId, FilterAdminSubordinationTree filter);
        #endregion

        #region [+] RegistrationJournalPositions ...
        IEnumerable<ITreeItem> GetRegistrationJournalPositionsDIP(IContext context, int positionId, FilterTree filter);
        IEnumerable<ITreeItem> GetPositionsByJournalDIP(IContext context, int journalId, FilterTree filter);
        #endregion

        IEnumerable<FrontPermission> GetUserPermissions(IContext context, FilterPermissionsAccess filter = null);
        IEnumerable<FrontModule> GetRolePermissions(IContext context, FilterAdminRolePermissionsDIP filter);
        FilterPermissionsAccess GetFilterPermissionsAccessByContext(IContext context, bool isPositionFromContext = true, List<int> permissionIDs = null, int? actionId = null, int? moduleId = null);
        bool ExistsPermissionsAccess(IContext context, FilterPermissionsAccess filter);
    }
}
