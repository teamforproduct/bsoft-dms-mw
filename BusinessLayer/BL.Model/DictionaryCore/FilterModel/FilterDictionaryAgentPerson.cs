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
        /// Список AgentCompanyId
        /// </summary>
        public List<int> AgentCompanyId { get; set; }

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

        public string NameExact { get; set; }

        public string FirstNameExact { get; set; }
        public string LastNameExact { get; set; }
        public string PassportSerial { get; set; }
        public int? PassportNumber { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
