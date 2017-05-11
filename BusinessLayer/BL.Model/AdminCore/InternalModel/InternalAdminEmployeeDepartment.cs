using BL.Model.Common;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminEmployeeDepartment : LastChangeInfo
    {

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Сотрудник
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Отдел
        /// </summary>
        public int DepartmentId { get; set; }

    }
}