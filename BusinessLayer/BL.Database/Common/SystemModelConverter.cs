using System.Collections.Generic;
using System.Linq;
using BL.Database.DBModel.Admin;
using BL.Model.Enums;
using BL.CrossCutting.Interfaces;
using System;
using BL.Database.DBModel.System;
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
                API = item.API,
                Category = item.Category,
                Code = item.Code,
                Description = item.Description,
                GrantId = item.GrantId,
                IsGrantable = item.IsGrantable,
                IsGrantableByRecordId = item.IsGrantableByRecordId,
                IsVisible = item.IsVisible,
                IsVisibleInMenu = item.IsVisibleInMenu,
                ObjectId = (int)item.ObjectId,
            };
        }

        public static SystemObjects GetDbSystemObject(IContext context, InternalSystemObject item)
        {
            return item == null ? null : new SystemObjects
            {
                Id = item.Id,
                Code = item.Code,
                Description = item.Description,
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