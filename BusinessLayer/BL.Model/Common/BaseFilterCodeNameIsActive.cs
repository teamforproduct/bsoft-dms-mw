namespace BL.Model.Common
{
    /// <summary>
    /// Базовые фильтры для справочников
    /// </summary>
    public class BaseFilterCodeNameIsActive : BaseFilterCodeName
    {
        /// <summary>
        /// Сужение по активности элементов
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
