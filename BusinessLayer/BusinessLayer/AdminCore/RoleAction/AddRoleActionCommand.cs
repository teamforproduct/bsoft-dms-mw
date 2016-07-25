using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class AddRoleActionCommand : BaseAdminCommand
    {

        private ModifyAdminRoleAction Model
        {
            get
            {
                if (!(_param is ModifyAdminRoleAction)) throw new WrongParameterTypeError();
                return (ModifyAdminRoleAction)_param;
            }
        }

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            var filter = new FilterAdminRoleAction
            {
                ActionIDs = new List<int> { Model.ActionId },
                RoleIDs = new List<int> { Model.RoleId }
            };

            if (Model.RecordId != null)
            {
                filter.RecordIDs = new List<int> { Model.RecordId ?? -1 };
            }

            if (_adminDb.ExistsRoleAction(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }

        public override object Execute()
        {
            try
            {
                var model = CommonAdminUtilities.RoleActionModifyToInternal(_context, Model);
                return _adminDb.AddRoleAction(_context, model);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}