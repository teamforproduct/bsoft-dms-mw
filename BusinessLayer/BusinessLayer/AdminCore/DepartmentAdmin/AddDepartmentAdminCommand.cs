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
                var model = new InternalDepartmentAdmin(Model);
                CommonDocumentUtilities.SetLastChange(_context, model);

                return _adminDb.AddDepartmentAdmin(_context, model);
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}