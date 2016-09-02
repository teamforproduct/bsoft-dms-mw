﻿using System.Collections.Generic;
using System.Linq;
using BL.Model.AdminCore.InternalModel;
using BL.Database.DBModel.Admin;
using BL.Model.Enums;
using BL.CrossCutting.Interfaces;

namespace BL.Database.Common
{
    public static class AdminModelConverter
    {

        public static AdminRoleTypes GetDbRoleType(IContext context, InternalAdminRoleType item)
        {
            return item == null ? null : new AdminRoleTypes
            {
                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                Code = item.Code,
                Name = item.Name
            };
        }
        public static AdminRoles GetDbRole(IContext context, InternalAdminRole item)
        {
            return item == null ? null : new AdminRoles
            {
                ClientId = context.CurrentClientId,

                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                RoleTypeId = item.RoleTypeId,
                Name = item.Name
            };
        }

        public static AdminRoleActions GetDbRoleAction(IContext context, InternalAdminRoleAction item)
        {
            return item == null ? null : new AdminRoleActions
            {
                Id = item.Id,
                LastChangeDate = item.LastChangeDate,
                LastChangeUserId = item.LastChangeUserId,
                ActionId = item.ActionId,
                RoleId = item.RoleId,
                RecordId = item.RecordId
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
                UserId = item.UserId,
                RoleId = item.RoleId,
                StartDate = item.StartDate,
                EndDate = item.EndDate
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

    }
}