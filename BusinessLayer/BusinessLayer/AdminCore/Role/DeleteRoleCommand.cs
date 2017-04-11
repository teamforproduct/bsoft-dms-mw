using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.Exception;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class DeleteRoleCommand : BaseAdminCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            string roleCode = _adminDb.GetRoleTypeCode(_context, Model);

            if (!string.IsNullOrEmpty(roleCode))
            {
                throw new DictionarySystemRecordCouldNotBeDeleted();
            }

            return true;
        }

        public override object Execute()
        {
            _adminDb.DeleteRolePermissions(_context, new FilterAdminRolePermissions { RoleIDs = new List<int> { Model } });
            _adminDb.DeletePositionRoles(_context, new FilterAdminPositionRole { RoleIDs = new List<int> { Model } });
            _adminDb.DeleteUserRoles(_context, new FilterAdminUserRole { RoleIDs = new List<int> { Model } });

            _adminDb.DeleteRoles(_context, new FilterAdminRole { IDs = new List<int> { Model } });
            return null;
        }
    }

}

