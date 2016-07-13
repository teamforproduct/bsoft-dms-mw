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
    public class FilterAdminPositionRole
    {

        /// <summary>
        /// Список ID
        /// </summary>
        public List<int> IDs { get; set; }

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