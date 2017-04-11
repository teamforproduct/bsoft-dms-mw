using BL.Model.AdminCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class DeleteDepartmentAdminCommand : BaseDepartmentAdminCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override object Execute()
        {
            _adminDb.DeleteDepartmentAdmins(_context, new FilterAdminEmployeeDepartments { IDs = new List<int> { Model } });
            return null;
        }
    }
}