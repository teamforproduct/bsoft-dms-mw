using BL.Model.Common;
using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр словаря физических лиц
    /// </summary>
    public class FilterDictionaryAgentPerson : DictionaryBaseFilterParameters
    {

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
        //public DateTime? BirthDate { get; set; }
        
        public Period BirthPeriod { get; set; }

        /// <summary>
        /// Пол (true - мужской)
        /// </summary>
        public bool? IsMale { get; set; }

        /// <summary>
        /// Первая буква наименования
        /// </summary>
        public char FirstChar { get; set; }
    }
}
