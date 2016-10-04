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
    public class ModifyUserRoleCommand : BaseUserRoleCommand
    {
        public override object Execute()
        {
            try
            {
                var dp = CommonAdminUtilities.UserRoleModifyToInternal(_context, Model);
                _adminDb.UpdateUserRole(_context, dp);
            }
            catch (AdminRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}