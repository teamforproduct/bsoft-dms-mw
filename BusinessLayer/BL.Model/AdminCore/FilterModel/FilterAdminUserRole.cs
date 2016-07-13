using BL.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminUserRole
    /// </summary>
    public class FilterAdminUserRole: AdminBaseFilterParameters
    {

        /// <summary>
        /// Пользователи
        /// </summary>
        public List<int> UserIDs { get; set; }

        /// <summary>
        /// Роли
        /// </summary>
        public List<int> RoleIDs { get; set; }

        /// <summary>
        /// Рассматриваемый период
        /// </summary>
        public Period Period { get; set; }

    }
}