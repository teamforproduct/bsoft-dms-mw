using BL.Model.Common;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр для типовой рассылки
    /// </summary>
    public class FilterDictionaryStandartSendList : BaseFilterNameIsActive
    {
        /// <summary>
        /// Должность
        /// </summary>
        public int? PositionID { get; set; }
        
    }
}
