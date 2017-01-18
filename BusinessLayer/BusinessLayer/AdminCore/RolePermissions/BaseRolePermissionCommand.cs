using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.AdminCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;


namespace BL.Logic.AdminCore
{
    public class BaseRolePermissionCommand : BaseAdminCommand
    {

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            return true;
        }

        protected void Set(InternalAdminRolePermission model, bool IsChecked)
        {
            CommonDocumentUtilities.SetLastChange(_context, model);

            var exists = _adminDb.ExistsRolePermissions(_context, new FilterAdminRolePermissions
            {
                RoleIDs = new List<int>() { model.RoleId },
                PermissionIDs = new List<int>() { model.PermissionId },
            });

            if (exists && !IsChecked) _adminDb.DeleteRolePermission(_context, model);
            else if (!exists && IsChecked) _adminDb.AddRolePermission(_context, model);
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}