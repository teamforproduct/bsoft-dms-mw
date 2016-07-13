using System.Collections.Generic;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтр словаря уровней доступа
    /// </summary>
    public class FilterAdminAccessLevel
    {
        /// <summary>
        /// Массив ИД уровней доступа
        /// </summary>
        public List<int> AccessLevelId { get; set; }
    }
}
