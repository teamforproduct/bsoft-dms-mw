using BL.Model.Common;

namespace BL.Model.Common
{
    /// <summary>
    /// Базовые фильтры для справочников
    /// </summary>
    public class BaseFilterNameIsActive : BaseFilterName
    {
        /// <summary>
        /// Сужение по активности элементов
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
