using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;


namespace BL.Logic.AdminCore
{
    public class BaseRoleCommand : BaseAdminCommand
    {
        private AddAdminRole Model { get { return GetModel<AddAdminRole>(); } }

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            var filter = new FilterAdminRole
            {
                NameExact = Model.Name
            };

            if (TypeModelIs<ModifyAdminRole>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyAdminRole>().Id }; }

            if (_adminDb.ExistsRole(_context, filter)) throw new AdminRecordNotUnique();

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}