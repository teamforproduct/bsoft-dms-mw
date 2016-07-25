using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class AddPositionRoleCommand : BaseAdminCommand
    {

        private ModifyAdminPositionRole Model
        {
            get
            {
                if (!(_param is ModifyAdminPositionRole)) throw new WrongParameterTypeError();
                return (ModifyAdminPositionRole)_param;
            }
        }

        public override bool CanBeDisplayed(int Id)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);

            var filter = new FilterAdminPositionRole { PositionIDs = new List<int> { Model.PositionId } , RoleIDs = new List<int> { Model.RoleId } };

            if (_adminDb.ExistsPositionRole(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }

        public override object Execute()
        {
            try
            {
                var model = CommonAdminUtilities.PositionRoleModifyToInternal(_context, Model);
                return _adminDb.AddPositionRole(_context, model);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}