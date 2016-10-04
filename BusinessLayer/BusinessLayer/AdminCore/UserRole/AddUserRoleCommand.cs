using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Common;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class AddUserRoleCommand : BaseUserRoleCommand
    {
        public override object Execute()
        {
            try
            {
                var model = CommonAdminUtilities.UserRoleModifyToInternal(_context, Model);
                return _adminDb.AddUserRole(_context, model);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}