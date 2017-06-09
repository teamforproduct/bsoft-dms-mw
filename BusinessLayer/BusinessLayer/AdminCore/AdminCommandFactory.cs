using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Verify;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.AdminCore
{
    public static class AdminCommandFactory
    {
        public static IAdminCommand GetAdminCommand(EnumActions act, IContext ctx, object param)
        {
            if (ctx.ClientLicence?.LicenceError != null)
            {
                throw ctx.ClientLicence.LicenceError as DmsExceptions;
            }

            IAdminCommand cmd;
            switch (act)
            {

                #region Roles
                case EnumActions.AddRole:
                    cmd = DmsResolver.Current.Get<AddRoleCommand>();
                    break;
                case EnumActions.ModifyRole:
                    cmd = DmsResolver.Current.Get<ModifyRoleCommand>();
                    break;
                case EnumActions.DeleteRole:
                    cmd = DmsResolver.Current.Get<DeleteRoleCommand>();
                    break;
                #endregion

                #region AdminRolePermission
                case EnumActions.SetRolePermission:
                    cmd = DmsResolver.Current.Get<SetRolePermissionCommand>();
                    break;
                case EnumActions.SetRolePermissionByModuleAccessType:
                    cmd = DmsResolver.Current.Get<SetRolePermissionByModuleAccessTypeCommand>();
                    break;
                case EnumActions.SetRolePermissionByModuleFeature:
                    cmd = DmsResolver.Current.Get<SetRolePermissionByModuleFeatureCommand>();
                    break;
                case EnumActions.SetRolePermissionByModule:
                    cmd = DmsResolver.Current.Get<SetRolePermissionByModuleCommand>();
                    break;
                #endregion

                #region AdminPositionRoles
                case EnumActions.SetPositionRole:
                    cmd = DmsResolver.Current.Get<SetPositionRoleCommand>();
                    break;
                case EnumActions.DuplicatePositionRoles:
                    cmd = DmsResolver.Current.Get<DuplicatePositionRolesCommand>();
                    break;

                #endregion

                #region AdminUserRoles
                case EnumActions.SetUserRole:
                    cmd = DmsResolver.Current.Get<SetUserRoleCommand>();
                    break;
                case EnumActions.SetUserRoleByAssignment:
                    cmd = DmsResolver.Current.Get<SetUserRoleByAssignmentCommand>();
                    break;
                #endregion

                #region AdminSubordinations
                //case EnumAdminActions.AddSubordination:
                //    cmd = DmsResolver.Current.Get<AddSubordinationCommand>();
                //    break;
                //case EnumAdminActions.ModifySubordination:
                //    cmd = DmsResolver.Current.Get<ModifySubordinationCommand>();
                //    break;
                //case EnumAdminActions.DeleteSubordination:
                //    cmd = DmsResolver.Current.Get<DeleteSubordinationCommand>();
                //    break;
                case EnumActions.SetSubordination:
                    cmd = DmsResolver.Current.Get<SetSubordinationCommand>();
                    break;
                case EnumActions.DuplicateSubordinations:
                    cmd = DmsResolver.Current.Get<DuplicateSubordinationsCommand>();
                    break;
                case EnumActions.SetSubordinationByDepartment:
                    cmd = DmsResolver.Current.Get<SetSubordinationByDepartmentCommand>();
                    break;
                case EnumActions.SetSubordinationByCompany:
                    cmd = DmsResolver.Current.Get<SetSubordinationByCompanyCommand>();
                    break;
                case EnumActions.SetDefaultSubordination:
                    cmd = DmsResolver.Current.Get<SetDefaultSubordinationsCommand>();
                    break;
                case EnumActions.SetAllSubordination:
                    cmd = DmsResolver.Current.Get<SetAllSubordinationsCommand>();
                    break;



                case EnumActions.AddDepartmentAdmin:
                    cmd = DmsResolver.Current.Get<AddDepartmentAdminCommand>();
                    break;
                case EnumActions.DeleteDepartmentAdmin:
                    cmd = DmsResolver.Current.Get<DeleteDepartmentAdminCommand>();
                    break;



                #endregion

                #region RegistrationJournalPosition
                case EnumActions.SetJournalAccess:
                    cmd = DmsResolver.Current.Get<SetJournalAccessCommand>();
                    break;
                case EnumActions.DuplicateJournalAccess_Journal:
                    cmd = DmsResolver.Current.Get<DuplicateJournalAccess_JournalCommand>();
                    break;
                case EnumActions.DuplicateJournalAccess_Position:
                    cmd = DmsResolver.Current.Get<DuplicateJournalAccess_PositionCommand>();
                    break;
                case EnumActions.SetJournalAccessByDepartment_Journal:
                    cmd = DmsResolver.Current.Get<SetJournalAccessByDepartment_JournalCommand>();
                    break;
                case EnumActions.SetJournalAccessByDepartment_Position:
                    cmd = DmsResolver.Current.Get<SetJournalAccessByDepartment_PositionCommand>();
                    break;
                case EnumActions.SetJournalAccessByCompany_Journal:
                    cmd = DmsResolver.Current.Get<SetJournalAccessByCompany_JournalCommand>();
                    break;
                case EnumActions.SetJournalAccessByCompany_Position:
                    cmd = DmsResolver.Current.Get<SetJournalAccessByCompany_PositionCommand>();
                    break;
                case EnumActions.SetJournalAccessDefault_Journal:
                    cmd = DmsResolver.Current.Get<SetJournalAccessDefault_JournalCommand>();
                    break;
                case EnumActions.SetJournalAccessDefault_Position:
                    cmd = DmsResolver.Current.Get<SetJournalAccessDefault_PositionCommand>();
                    break;
                case EnumActions.SetJournalAccessAll_Journal:
                    cmd = DmsResolver.Current.Get<SetJournalAccessAll_JournalCommand>();
                    break;
                case EnumActions.SetJournalAccessAll_Position:
                    cmd = DmsResolver.Current.Get<SetJournalAccessAll_PositionCommand>();
                    break;
                
                #endregion


                default:
                    throw new CommandNotDefinedError(act.ToString());
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}