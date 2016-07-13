using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminRoleAction
    /// </summary>
    public class FilterAdminRoleAction
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
        /// Действия
        /// </summary>
        public List<int> ActionIDs { get; set; }

        /// <summary>
        /// Записи
        /// </summary>
        public List<int> RecordIDs { get; set; }
    }
}