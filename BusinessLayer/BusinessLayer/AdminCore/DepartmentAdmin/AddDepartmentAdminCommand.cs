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
        private AddAdminDepartmentAdmin Model
        {
            get
            {
                if (!(_param is AddAdminDepartmentAdmin)) throw new WrongParameterTypeError();
                return (AddAdminDepartmentAdmin)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                var da = new InternalDepartmentAdmin(Model);
                //da.EmployeeName = _dictDb.GetAgents(_context, new BL.Model.DictionaryCore.FilterModel.FilterDictionaryAgent { IDs = new List<int> { Model.EmployeeId } }, null). .Name;


                return _adminDb.AddDepartmentAdmin(_context, da);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}