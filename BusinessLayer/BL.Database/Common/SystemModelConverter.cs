﻿using BL.CrossCutting.Interfaces;
using BL.Database.DBModel.System;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;

namespace BL.Database.Common
{
    public static class SystemModelConverter
    {


        public static SystemActions GetDbSystemAction(IContext context, InternalSystemAction item)
        {
            return item == null ? null : new SystemActions
            {
                Id = item.Id,
                CategoryId = (int?) item.Category,
                Code = ((EnumActions)item.Id).ToString(),
                //Description = item.Description,
                ObjectId = (int)item.ObjectId,
                PermissionId = item.PermissionId,
            };
        }

        public static SystemObjects GetDbSystemObject(IContext context, InternalSystemObject item)
        {
            return item == null ? null : new SystemObjects
            {
                Id = item.Id,
                Code = item.Code,
            };
        }

        //public static IEnumerable<AdminRoleActions> GetDbRoleActions(IContext context, IEnumerable<InternalAdminRoleAction> list)
        //{
        //    {
        //        var items = new List<AdminRoleActions>();
        //        foreach (var item in list)
        //        {
        //            items.Add(GetDbRoleAction(context, item));
        //        }
        //        return items;
        //    }
        //}



    }
}