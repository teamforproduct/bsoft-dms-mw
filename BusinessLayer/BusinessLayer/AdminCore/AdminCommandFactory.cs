﻿using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.AdminCore.Verify;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.AdminCore
{
    public static class AdminCommandFactory
    {
        public static IAdminCommand GetAdminCommand(EnumAdminActions act, IContext ctx, object param)
        {
            if (ctx.ClientLicence?.LicenceError != null)
            {
                throw ctx.ClientLicence.LicenceError as DmsExceptions;
            }

            IAdminCommand cmd;
            switch (act)
            {

                #region Roles
                case EnumAdminActions.AddRole:
                    cmd = DmsResolver.Current.Get<AddRoleCommand>();
                    break;
                case EnumAdminActions.ModifyRole:
                    cmd = DmsResolver.Current.Get<ModifyRoleCommand>();
                    break;
                case EnumAdminActions.DeleteRole:
                    cmd = DmsResolver.Current.Get<DeleteRoleCommand>();
                    break;
                #endregion

                #region AdminRolePermission
                case EnumAdminActions.SetRolePermission:
                    cmd = DmsResolver.Current.Get<SetRolePermissionCommand>();
                    break;
                case EnumAdminActions.SetRolePermissionByModuleAccessType:
                    cmd = DmsResolver.Current.Get<SetRolePermissionByModuleAccessTypeCommand>();
                    break;
                case EnumAdminActions.SetRolePermissionByModuleFeature:
                    cmd = DmsResolver.Current.Get<SetRolePermissionByModuleFeatureCommand>();
                    break;
                case EnumAdminActions.SetRolePermissionByModule:
                    cmd = DmsResolver.Current.Get<SetRolePermissionByModuleCommand>();
                    break;
                #endregion

                #region AdminPositionRoles
                case EnumAdminActions.SetPositionRole:
                    cmd = DmsResolver.Current.Get<SetPositionRoleCommand>();
                    break;
                case EnumAdminActions.DuplicatePositionRoles:
                    cmd = DmsResolver.Current.Get<DuplicatePositionRolesCommand>();
                    break;

                #endregion

                #region AdminUserRoles
                case EnumAdminActions.SetUserRole:
                    cmd = DmsResolver.Current.Get<SetUserRoleCommand>();
                    break;
                case EnumAdminActions.SetUserRoleByAssignment:
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
                case EnumAdminActions.SetSubordination:
                    cmd = DmsResolver.Current.Get<SetSubordinationCommand>();
                    break;
                case EnumAdminActions.DuplicateSubordinations:
                    cmd = DmsResolver.Current.Get<DuplicateSubordinationsCommand>();
                    break;
                case EnumAdminActions.SetSubordinationByDepartment:
                    cmd = DmsResolver.Current.Get<SetSubordinationByDepartmentCommand>();
                    break;
                case EnumAdminActions.SetSubordinationByCompany:
                    cmd = DmsResolver.Current.Get<SetSubordinationByCompanyCommand>();
                    break;
                case EnumAdminActions.SetDefaultSubordination:
                    cmd = DmsResolver.Current.Get<SetDefaultSubordinationsCommand>();
                    break;
                case EnumAdminActions.SetAllSubordination:
                    cmd = DmsResolver.Current.Get<SetAllSubordinationsCommand>();
                    break;



                case EnumAdminActions.AddDepartmentAdmin:
                    cmd = DmsResolver.Current.Get<AddDepartmentAdminCommand>();
                    break;
                case EnumAdminActions.DeleteDepartmentAdmin:
                    cmd = DmsResolver.Current.Get<DeleteDepartmentAdminCommand>();
                    break;



                #endregion

                #region RegistrationJournalPosition
                case EnumAdminActions.SetRegistrationJournalPosition:
                    cmd = DmsResolver.Current.Get<SetRJournalPositionCommand>();
                    break;
                case EnumAdminActions.DuplicateRegistrationJournalPositions:
                    cmd = DmsResolver.Current.Get<DuplicateRJournalPositionCommand>();
                    break;
                case EnumAdminActions.SetRegistrationJournalPositionByDepartment:
                    cmd = DmsResolver.Current.Get<SetRJournalPositionByDepartmentCommand>();
                    break;
                case EnumAdminActions.SetRegistrationJournalPositionByCompany:
                    cmd = DmsResolver.Current.Get<SetRJournalPositionByCompanyCommand>();
                    break;
                case EnumAdminActions.SetDefaultRegistrationJournalPosition:
                    cmd = DmsResolver.Current.Get<SetDefaultRJournalPositionsCommand>();
                    break;
                case EnumAdminActions.SetAllRegistrationJournalPosition:
                    cmd = DmsResolver.Current.Get<SetAllRJournalPositionsCommand>();
                    break;
                #endregion

                #region AdminSubordinations
                case EnumAdminActions.ChangePassword:
                case EnumAdminActions.ChangeLockout:
                case EnumAdminActions.KillSessions:
                case EnumAdminActions.ChangeLogin:
                case EnumAdminActions.MustChangePassword:
                    cmd = DmsResolver.Current.Get<VerifyAccessCommand>();
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