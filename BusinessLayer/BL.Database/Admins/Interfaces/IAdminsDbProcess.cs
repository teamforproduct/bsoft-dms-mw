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

namespace BL.Database.Admins.Interfaces
{
    public interface IAdminsDbProcess
    {
        #region [+] General ...
        AdminAccessInfo GetAdminAccesses(IContext context);
        Employee GetEmployee(IContext ctx, string userId);

        IEnumerable<FrontAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter);
        Dictionary<int, int> GetCurrentPositionsAccessLevel(IContext context);

        IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee);
        #endregion

        bool VerifySubordination(IContext context, VerifySubordination model);

        #region [+] Roles ...
        int AddRoleType(IContext context, InternalAdminRoleType model);
        int AddNamedRole(IContext context, string code, string name, IEnumerable<InternalAdminRoleAction> roleActions);
        int AddRole(IContext context, InternalAdminRole model);
        void UpdateRole(IContext context, InternalAdminRole model);
        void DeleteRole(IContext context, InternalAdminRole model);
        bool ExistsRole(IContext context, FilterAdminRole filter);
        InternalAdminRole GetInternalRole(IContext context, FilterAdminRole filter);
        IEnumerable<FrontAdminRole> GetRoles(IContext context, FilterAdminRole filter);
        #endregion

        #region [+] RoleAction ...
        int AddRoleAction(IContext context, InternalAdminRoleAction model);
        void UpdateRoleAction(IContext context, InternalAdminRoleAction model);
        void DeleteRoleAction(IContext context, InternalAdminRoleAction model);
        bool ExistsRoleAction(IContext context, FilterAdminRoleAction filter);
        InternalAdminRoleAction GetInternalRoleAction(IContext context, FilterAdminRoleAction filter);
        IEnumerable<FrontAdminRoleAction> GetRoleActions(IContext context, FilterAdminRoleAction filter);
        #endregion

        #region [+] PositionRole ...
        int AddPositionRole(IContext context, InternalAdminPositionRole model);
        void UpdatePositionRole(IContext context, InternalAdminPositionRole model);
        void DeletePositionRole(IContext context, InternalAdminPositionRole model);
        bool ExistsPositionRole(IContext context, FilterAdminPositionRole filter);
        InternalAdminPositionRole GetInternalPositionRole(IContext context, FilterAdminPositionRole filter);

        FrontAdminPositionRole GetPositionRole(IContext context, int id);
        IEnumerable<FrontAdminPositionRole> GetPositionRoles(IContext context, FilterAdminRole filter);
        #endregion

        #region [+] UserRole ...
        int AddUserRole(IContext context, InternalAdminUserRole model);
        void UpdateUserRole(IContext context, InternalAdminUserRole model);
        void DeleteUserRole(IContext context, InternalAdminUserRole model);
        bool ExistsUserRole(IContext context, FilterAdminUserRole filter);
        InternalAdminUserRole GetInternalUserRole(IContext context, FilterAdminUserRole filter);
        IEnumerable<FrontAdminUserRole> GetUserRoles(IContext context, FilterAdminUserRole filter);
        #endregion

        IEnumerable<FrontDictionaryAgentEmployee> GetDepartmentAdmins(IContext context, int departmentId);

        #region [+] Subordination ...
        int AddSubordination(IContext context, InternalAdminSubordination model);
        void UpdateSubordination(IContext context, InternalAdminSubordination model);
        void DeleteSubordination(IContext context, InternalAdminSubordination model);
        void DeleteSubordinationsBySourcePositionId(IContext context, InternalAdminSubordination model);
        void DeleteSubordinationsByTargetPositionId(IContext context, InternalAdminSubordination model);
        bool ExistsSubordination(IContext context, FilterAdminSubordination filter);
        List<int> GetSubordinationTargetIDs(IContext context, FilterAdminSubordination filter);
        InternalAdminSubordination GetInternalSubordination(IContext context, FilterAdminSubordination filter);
        IEnumerable<FrontAdminSubordination> GetSubordinations(IContext context, FilterAdminSubordination filter);
        #endregion

        #region [+] MainMenu ...
        IEnumerable<TreeItem> GetMainMenu(IContext context);
        #endregion

        #region [+] AddNewClient ...

        List<InternalAdminRoleAction> GetRoleActionsForAdmin(IContext context);

        #endregion

    }
}