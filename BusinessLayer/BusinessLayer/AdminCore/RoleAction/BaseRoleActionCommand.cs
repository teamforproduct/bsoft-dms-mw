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
    public class BaseRoleActionCommand : BaseAdminCommand
    {

        protected ModifyAdminRoleAction Model
        {
            get
            {
                if (!(_param is ModifyAdminRoleAction)) throw new WrongParameterTypeError();
                return (ModifyAdminRoleAction)_param;
            }
        }

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            var filter = new FilterAdminRoleAction
            {
                NotContainsIDs = new List<int> { Model.Id },
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
        { throw new NotImplementedException(); }
    }
}