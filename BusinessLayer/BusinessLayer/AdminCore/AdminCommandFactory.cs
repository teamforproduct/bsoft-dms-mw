﻿using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
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

                #region AdminRoleActions
                case EnumAdminActions.AddRoleAction:
                    cmd = DmsResolver.Current.Get<AddRoleActionCommand>();
                    break;
                case EnumAdminActions.ModifyRoleAction:
                    cmd = DmsResolver.Current.Get<ModifyRoleActionCommand>();
                    break;
                case EnumAdminActions.DeleteRoleAction:
                    cmd = DmsResolver.Current.Get<DeleteRoleActionCommand>();
                    break;
                #endregion
                    
                #region AdminPositionRoles
                case EnumAdminActions.AddPositionRole:
                    cmd = DmsResolver.Current.Get<AddPositionRoleCommand>();
                    break;
                case EnumAdminActions.ModifyPositionRole:
                    cmd = DmsResolver.Current.Get<ModifyPositionRoleCommand>();
                    break;
                case EnumAdminActions.DeletePositionRole:
                    cmd = DmsResolver.Current.Get<DeletePositionRoleCommand>();
                    break;
                #endregion

                #region AdminUserRoles
                case EnumAdminActions.AddUserRole:
                    cmd = DmsResolver.Current.Get<AddUserRoleCommand>();
                    break;
                case EnumAdminActions.ModifyUserRole:
                    cmd = DmsResolver.Current.Get<ModifyUserRoleCommand>();
                    break;
                case EnumAdminActions.DeleteUserRole:
                    cmd = DmsResolver.Current.Get<DeleteUserRoleCommand>();
                    break;
                #endregion

                #region AdminSubordinations
                case EnumAdminActions.AddSubordination:
                    cmd = DmsResolver.Current.Get<AddSubordinationCommand>();
                    break;
                case EnumAdminActions.ModifySubordination:
                    cmd = DmsResolver.Current.Get<ModifySubordinationCommand>();
                    break;
                case EnumAdminActions.DeleteSubordination:
                    cmd = DmsResolver.Current.Get<DeleteSubordinationCommand>();
                    break;
                #endregion

                default:
                    throw new CommandNotDefinedError();
            }
            cmd.InitializeCommand(act, ctx, param);
            return cmd;
        }
    }
}