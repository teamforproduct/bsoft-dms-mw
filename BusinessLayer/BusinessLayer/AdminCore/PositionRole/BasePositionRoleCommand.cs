using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class BasePositionRoleCommand : BaseAdminCommand
    {

        protected ModifyAdminPositionRole Model
        {
            get
            {
                if (!(_param is ModifyAdminPositionRole)) throw new WrongParameterTypeError();
                return (ModifyAdminPositionRole)_param;
            }
        }

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            var filter = new FilterAdminPositionRole
            {
                NotContainsIDs = new List<int> { Model.Id },
                PositionIDs = new List<int> { Model.PositionId },
                RoleIDs = new List<int> { Model.RoleId }
            };

            if (_adminDb.ExistsPositionRole(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }


        public override object Execute()
        { throw new NotImplementedException(); }

    }
}