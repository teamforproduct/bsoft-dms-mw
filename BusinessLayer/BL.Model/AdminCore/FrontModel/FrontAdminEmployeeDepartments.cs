using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    public class FrontAdminEmployeeDepartments 
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
        /// Сотрудник
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// Отдел
        /// </summary>
        public int DepartmentId { get; set; }


    }
}