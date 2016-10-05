using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class AddDepartmentAdminCommand : BaseDepartmentAdminCommand
    {
        public override object Execute()
        {
            try
            {
                var da = new InternalDepartmentAdmin(Model);
                da.EmployeeName = _dictDb.GetAgent(_context, Model.EmployeeId).Name;


                return _adminDb.AddDepartmentAdmin(_context, da);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}