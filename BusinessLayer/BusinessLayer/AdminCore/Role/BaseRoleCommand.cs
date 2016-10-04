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
    public class BaseRoleCommand : BaseAdminCommand
    {

        protected ModifyAdminRole Model
        {
            get
            {
                if (!(_param is ModifyAdminRole)) throw new WrongParameterTypeError();
                return (ModifyAdminRole)_param;
            }
        }

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            var filter = new FilterAdminRole
            {
                NotContainsIDs = new List<int> { Model.Id },
                NameExact = Model.Name
            };

            if (_adminDb.ExistsRole(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}