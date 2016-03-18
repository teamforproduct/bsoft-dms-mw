using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр словаря физических лиц
    /// </summary>
    public class FilterDictionaryAgentPerson
    {
        /// <summary>
        /// Массив ИД контрагентов
        /// </summary>
        public List<int> AgentId { get; set; }
        /// <summary>
        /// Отрывок из ФИО
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Отрывок из паспортных данных
        /// </summary>
        public string Passport { get; set; }
        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// Пол (true - мужской)
        /// </summary>
        public bool? IsMale { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// игнорировать при поиске
        /// </summary>
        public List<int> NotContainsId { get; set; }

    }
}
