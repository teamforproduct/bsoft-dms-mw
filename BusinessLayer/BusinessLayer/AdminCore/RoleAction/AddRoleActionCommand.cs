﻿using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class AddRoleActionCommand : BaseRoleActionCommand
    {
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