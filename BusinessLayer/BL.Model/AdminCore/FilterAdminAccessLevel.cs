using BL.Model.AdminCore.FilterModel;
using System.Collections.Generic;

namespace BL.Model.AdminCore
{
    /// <summary>
    /// Фильтр словаря уровней доступа
    /// </summary>
    public class FilterAdminAccessLevel: AdminBaseFilterParameters
    {

        /// <summary>
        /// Сужение по коду (вхождение)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Сужение по коду (равенство)
        /// </summary>
        public string CodeExact { get; set; }

        /// <summary>
        /// Сужение по наименованию (вхождение)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сужение по наименованию (равенство)
        /// </summary>
        public string NameExact { get; set; }
    }

}
