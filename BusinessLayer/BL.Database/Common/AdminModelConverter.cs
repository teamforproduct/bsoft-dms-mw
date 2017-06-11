using System.Collections.Generic;
using System.Linq;
using BL.Model.AdminCore.InternalModel;
using BL.Database.DBModel.Admin;
using BL.Model.Enums;
using BL.CrossCutting.Interfaces;
using System;

namespace BL.Database.Common
{
    public static class AdminModelConverter
    {

        public static AdminRoleTypes GetDbRoleType(IContext context, InternalAdminRoleType item)
        {
            return item == null ? null : new AdminRoleTypes
            {
                Id = item.Id,
                Code = ((EnumRoleTypes)item.Id).ToString(),
            };
        }
        public static AdminRoles GetDbRole(IContext context, InternalAdminRole item)
        {
            return item == null ? null : new AdminRoles
            {
                ClientId = context.Client.Id,

                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                RoleTypeId = item.RoleTypeId,
                Name = item.Name,
                Description = item.Description,
            };
        }



        public static AdminPositionRoles GetDbPositionRole(IContext context, InternalAdminPositionRole item)
        {
            return item == null ? null : new AdminPositionRoles
            {
                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                PositionId = item.PositionId,
                RoleId = item.RoleId
            };
        }

        public static AdminUserRoles GetDbUserRole(IContext context, InternalAdminUserRole item)
        {
            return item == null ? null : new AdminUserRoles
            {
                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                RoleId = item.RoleId,
                PositionExecutorId = item.PositionExecutorId,
                //PositionId = item.PositionId,
                //StartDate = item.StartDate,
                //EndDate = item.EndDate ?? DateTime.MaxValue
            };
        }

        public static IEnumerable<AdminUserRoles> GetDbUserRoles(IContext context, IEnumerable<InternalAdminUserRole> list)
        {
            var items = new List<AdminUserRoles>();
            foreach (var item in list)
            {
                items.Add(GetDbUserRole(context, item));
            }
            return items;
        }

        public static AdminEmployeeDepartments GetDbEmployeeDepartments(IContext context, InternalDepartmentAdmin item)
        {
            return item == null ? null : new AdminEmployeeDepartments
            {
                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                EmployeeId = item.EmployeeId,
                DepartmentId = item.DepartmentId,
            };
        }

        public static AdminSubordinations GetDbSubordination(IContext context, InternalAdminSubordination item)
        {
            return item == null ? null : new AdminSubordinations
            {
                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                SourcePositionId = item.SourcePositionId,
                TargetPositionId = item.TargetPositionId,
                SubordinationTypeId = item.SubordinationTypeId
            };
        }

        public static List<AdminSubordinations> GetDbSubordinations(IContext context, List<InternalAdminSubordination> list)
        {
            var items = new List<AdminSubordinations>();

            foreach (var item in list)
            {
                items.Add(GetDbSubordination(context, item));
            }

            return items;
        }
        public static AdminRolePermissions GetDbRolePermission(IContext context, InternalAdminRolePermission item)
        {
            return item == null ? null : new AdminRolePermissions
            {
                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                RoleId = item.RoleId,
                PermissionId = item.PermissionId,
            };
        }

        public static IEnumerable<AdminRolePermissions> GetDbRolePermissions(IContext context, IEnumerable<InternalAdminRolePermission> list)
        {
            {
                var items = new List<AdminRolePermissions>();
                foreach (var item in list)
                {
                    items.Add(GetDbRolePermission(context, item));
                }
                return items;
            }
        }

        public static AdminRegistrationJournalPositions GetDbRegistrationJournalPosition(IContext context, InternalRegistrationJournalPosition item)
        {
            return item == null ? null : new AdminRegistrationJournalPositions
            {
                Id = item.Id,
                PositionId = item.PositionId,
                RegJournalId = item.RegistrationJournalId,
                RegJournalAccessTypeId = item.RegJournalAccessTypeId
            };
        }

        public static List<AdminRegistrationJournalPositions> GetDbRegistrationJournalPositions(IContext context, List<InternalRegistrationJournalPosition> list)
        {
            var items = new List<AdminRegistrationJournalPositions>();

            foreach (var item in list)
            {
                items.Add(GetDbRegistrationJournalPosition(context, item));
            }

            return items;
        }


    }
}