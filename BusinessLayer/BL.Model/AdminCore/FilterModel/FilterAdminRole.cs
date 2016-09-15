using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminRole
    /// </summary>
    public class FilterAdminRole: AdminBaseFilterParameters
    {

        /// <summary>
        /// Сужение по наименованию элементов (по входжению)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сужение по наименованию элементов (по равенству)
        /// </summary>
        public string NameExact { get; set; }

        /// <summary>
        /// Сужение по ролям
        /// </summary>
        public List<int> RoleIDs { get; set; }

        /// <summary>
        /// Сужение по типам
        /// </summary>
        [IgnoreDataMember]
        public List<int> RoleTypeIDs { get; set; }

        /// <summary>
        /// Список должностей
        /// </summary>
        public List<int> PositionIDs { get; set; }

        /// <summary>
        /// Список ключей из таблицы AdminPositionRoles
        /// </summary>
        [IgnoreDataMember]
        public List<int> LinkIDs { get; set; }

        /// <summary>
        /// Отмечнено
        /// </summary>
        public bool? IsChecked { get; set; }

    }
}