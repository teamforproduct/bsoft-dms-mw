using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class BaseRolePermissionCommand : BaseAdminCommand
    {

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        { throw new NotImplementedException(); }

        public bool IsModified(int roleId)
        {
            string roleCode = _adminDb.GetRoleTypeCode(_context, roleId);

            if (!string.IsNullOrEmpty(roleCode)) throw new DefaultRolesCouldNotBeModified();

            return true;
        }

        protected void Set(string module, string feature, string accessType, int roleId, bool IsChecked)
        {
            var permissionId = _systemDb.GetPermissionId(_context, module, feature, accessType);

            if (permissionId < 0) throw new AdminRecordWasNotFound();

            var model = new InternalAdminRolePermission { PermissionId = permissionId , RoleId = roleId };
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