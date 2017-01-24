using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class AddRoleCommand : BaseRoleCommand
    {
        private AddAdminRole Model { get { return GetModel<AddAdminRole>(); } }

        public override object Execute()
        {
            try
            {
                var model = new InternalAdminRole(Model);
                CommonDocumentUtilities.SetLastChange(_context, model);
                return _adminDb.AddRole(_context, model);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}