using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр для типовой рассылки
    /// </summary>
    public class FilterDictionaryStandartSendList : DictionaryBaseFilterParameters
    {
        /// <summary>
        /// Должность
        /// </summary>
        public int? PositionID { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        
    }
}
