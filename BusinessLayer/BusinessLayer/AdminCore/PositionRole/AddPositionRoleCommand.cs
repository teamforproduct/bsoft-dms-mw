using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class AddPositionRoleCommand : BasePositionRoleCommand
    {
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