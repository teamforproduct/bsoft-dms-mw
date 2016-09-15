using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminPositionRole
    /// </summary>
    public class FilterAdminPositionRole: AdminBaseFilterParameters
    {

      
        /// <summary>
        /// Список ролей
        /// </summary>
        public List<int> RoleIDs { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public List<int> PositionIDs { get; set; }

    }
}