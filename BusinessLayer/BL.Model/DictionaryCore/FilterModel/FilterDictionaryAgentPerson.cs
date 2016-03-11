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
        public string FullName { get; set; }
        /// <summary>
        /// Отрывок из паспортных данных
        /// </summary>
        public string Passport { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Пол (true - мужской)
        /// </summary>
        public bool? IsMale { get; set; }
    }
}
