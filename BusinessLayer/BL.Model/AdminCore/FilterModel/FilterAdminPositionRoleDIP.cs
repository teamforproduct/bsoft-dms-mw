using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminRole
    /// </summary>
    public class FilterAdminPositionRoleDIP: AdminBaseFilterParameters
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
        /// Список должностей
        /// </summary>
        public List<int> PositionIDs { get; set; }

        /// <summary>
        /// Отмечнено
        /// </summary>
        [IgnoreDataMember]
        public bool IsChecked { get; set; }

    }
}