using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore;
using BL.Model.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Users;
using System.Collections.Generic;
using BL.Model.Tree;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.SystemCore;
using BL.Model.Enums;

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        #region [+] General ...
        AdminAccessInfo GetAdminAccesses(IContext context);
        Employee GetEmployeeForContext(IContext ctx, string userId);

        IEnumerable<FrontUserAssignments> GetAvailablePositions(IContext ctx, int agentId, List<int> PositionIDs);
        Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext context);

        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);
        #endregion

        bool VerifySubordination(IContext context, VerifySubordination model);

        #region [+] Roles ...
        int AddRoleType(IContext context, InternalAdminRoleType model);
        int AddRole(IContext context, InternalAdminRole model);
        void UpdateRole(IContext context, InternalAdminRole model);
        void DeleteRole(IContext context, int id);
        bool ExistsRole(IContext context, FilterAdminRole filter);
        string GetRoleTypeCode(IContext context, int id);
        int GetRoleByCode(IContext context, Roles item);
        InternalAdminRole GetInternalRole(IContext context, FilterAdminRole filter);
        IEnumerable<ListItem> GetListRoles(IContext context, FilterAdminRole filter, UIPaging paging);
        IEnumerable<FrontAdminRole> GetRoles(IContext context, FilterAdminRole filter);
        #endregion


        #region [+] PositionRole ...
        int AddPositionRole(IContext context, InternalAdminPositionRole model);
        void DeletePositionRole(IContext context, InternalAdminPositionRole model);
        void DeletePositionRoles(IContext context, FilterAdminPositionRole filter);
        bool ExistsPositionRole(IContext context, FilterAdminPositionRole filter);
        InternalAdminPositionRole GetInternalPositionRole(IContext context, FilterAdminPositionRole filter);
        FrontAdminPositionRole GetPositionRole(IContext context, int id);
        IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext context, FilterAdminPositionRole filter);
        IEnumerable<FrontAdminPositionRole> GetPositionRolesDIP(IContext context, FilterAdminPositionRoleDIP filter);
        //IEnumerable<FrontDIPUserRolesRoles> GetPositionRolesDIPUserRoles(IContext context, FilterAdminPositionRole filter);
        #endregion

        #region [+] UserRole ...
        int AddUserRole(IContext context, InternalAdminUserRole model);
        void AddUserRoles(IContext context, IEnumerable<InternalAdminUserRole> models);
        void UpdateUserRole(IContext context, InternalAdminUserRole model);
        void DeleteUserRole(IContext context, int id);
        void DeleteUserRoles(IContext context, FilterAdminUserRole filter);
        bool ExistsUserRole(IContext context, FilterAdminUserRole filter);
        IEnumerable<InternalAdminUserRole> GetInternalUserRoles(IContext context, FilterAdminUserRole filter);
        IEnumerable<InternalAdminPositionRole> GetInternalPositionRoles(IContext context, FilterAdminPositionRole filter);
        IEnumerable<FrontAdminUserRole> GetUserRoles(IContext context, FilterAdminUserRole filter);
        List<int> GetRolesByUsers(IContext context, FilterAdminUserRole filter);
        #endregion

        IEnumerable<FrontAdminEmployeeDepartments> GetDepartmentAdmins(IContext context, int departmentId);

        #region [+] Subordination ...
        int AddSubordination(IContext context, InternalAdminSubordination model);
        void AddSubordinations(IContext context, List<InternalAdminSubordination> list);
        void UpdateSubordination(IContext context, InternalAdminSubordination model);
        void DeleteSubordination(IContext context, InternalAdminSubordination model);
        void DeleteSubordinationsBySourcePositionId(IContext context, InternalAdminSubordination model);
        void DeleteSubordinationsByTargetPositionId(IContext context, InternalAdminSubordination model);
        void DeleteSubordinations(IContext context, FilterAdminSubordination filter);
        bool ExistsSubordination(IContext context, FilterAdminSubordination filter);
        List<int> GetSubordinationTargetIDs(IContext context, FilterAdminSubordination filter);
        InternalAdminSubordination GetInternalSubordination(IContext context, FilterAdminSubordination filter);
        IEnumerable<FrontAdminSubordination> GetSubordinations(IContext context, FilterAdminSubordination filter);
        IEnumerable<InternalAdminSubordination> GetInternalSubordinations(IContext context, FilterAdminSubordination filter);
        #endregion


        #region [+] RegistrationJournalPosition ...
        int AddRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition model);
        void AddRegistrationJournalPositions(IContext context, List<InternalRegistrationJournalPosition> list);
        void UpdateRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition model);
        void DeleteRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition model);
        //void DeleteRegistrationJournalPositionsBySourcePositionId(IContext context, InternalRegistrationJournalPosition model);
        //void DeleteRegistrationJournalPositionsByTargetPositionId(IContext context, InternalRegistrationJournalPosition model);
        void DeleteRegistrationJournalPositions(IContext context, FilterAdminRegistrationJournalPosition filter);
        bool ExistsRegistrationJournalPosition(IContext context, FilterAdminRegistrationJournalPosition filter);
        InternalRegistrationJournalPosition GetInternalRegistrationJournalPosition(IContext context, FilterAdminRegistrationJournalPosition filter);
        //IEnumerable<FrontAdminRegistrationJournalPosition> GetRegistrationJournalPositions(IContext context, FilterAdminRegistrationJournalPosition filter);
        IEnumerable<InternalRegistrationJournalPosition> GetInternalRegistrationJournalPositions(IContext context, FilterAdminRegistrationJournalPosition filter);
        #endregion

        #region [+] AddNewClient ...

        List<InternalAdminRolePermission> GetRolePermissionsForAdmin(IContext context);

        #endregion

        int AddDepartmentAdmin(IContext context, InternalDepartmentAdmin model);

        void DeleteDepartmentAdmin(IContext context, int id);

        int AddRolePermission(IContext context, InternalAdminRolePermission model);
        void AddRolePermissions(IContext context, IEnumerable<InternalAdminRolePermission> models);

        void DeleteRolePermission(IContext context, InternalAdminRolePermission model);

        bool ExistsRolePermissions(IContext context, FilterAdminRolePermissions filter);
        IEnumerable<FrontPermission> GetUserPermissionsAccess(IContext context, FilterPermissionsAccess filter);
        bool ExistsPermissionsAccess(IContext context, FilterPermissionsAccess filter);

        IEnumerable<FrontModule> GetRolePermissions(IContext context, FilterAdminRolePermissionsDIP filter);

    }
}