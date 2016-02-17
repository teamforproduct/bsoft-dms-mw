using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    /// <summary>
    /// Фильтр словаря агентов
    /// </summary>
    public class FilterDictionaryAgent
    {
        /// <summary>
        /// Массив ИД агентов
        /// </summary>
        public List<int> Id { get; set; }
        /// <summary>
        /// Отрывок наименования агента
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Налоговый код агента
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// Признак является ли агент физическим лицом
        /// </summary>
        public bool? IsIndividual { get; set; }
        /// <summary>
        /// Признак является ли агент сотрудником
        /// </summary>
        public bool? IsEmployee { get; set; }
    }
}
