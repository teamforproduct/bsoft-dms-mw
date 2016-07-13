using System.Collections.Generic;
using System.Linq;
using BL.Model.AdminCore.InternalModel;
using BL.Database.DBModel.Admin;
using BL.Model.Enums;
using BL.CrossCutting.Interfaces;

namespace BL.Database.Common
{
    public static class AdminModelConverter
    {

        public static AdminPositionRoles GetDbPositionRole(InternalAdminPositionRole item)
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

    }
}