using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalDepartmentAdmin : LastChangeInfo
    {
        public InternalDepartmentAdmin()
        { }

        public InternalDepartmentAdmin(AddAdminDepartmentAdmin model)
        {
            DepartmentId = model.DepartmentId;
            EmployeeId = model.EmployeeId;
        }

        /// <summary>
        /// Отдел
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Сотрудник
        /// </summary>
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

    }
}