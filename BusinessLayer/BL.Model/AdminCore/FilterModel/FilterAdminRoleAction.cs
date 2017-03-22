using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminRoleAction
    /// </summary>
    public class FilterAdminRoleAction: BaseFilter
    {

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