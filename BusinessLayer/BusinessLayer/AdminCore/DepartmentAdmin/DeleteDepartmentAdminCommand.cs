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
    public class DeleteDepartmentAdminCommand : BaseDepartmentAdminCommand
    {
        public override object Execute()
        {
            try
            {
                //var model = CommonAdminUtilities.PositionRoleModifyToInternal(_context, Model);
                //_adminDb.UpdatePositionRole(_context, model);
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