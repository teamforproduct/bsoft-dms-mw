using BL.Model.Common;
using BL.Model.Extensions;
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
        /// Строка, для полнотекстового поиска
        /// </summary>
        public string FullTextSearchString { get; set; }

        /// <summary>
        /// Ссылка на организацию для контактных лиц
        /// </summary>
        public List<int> CompanyIDs{ get; set; }

        /// <summary>
        /// Отрывок из паспортных данных
        /// </summary>
        public string Passport { get; set; }
        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxCodeExact { get; set; }

        /// <summary>
        /// Период рождения
        /// </summary>
        public Period BirthPeriod { get; set; }

        /// <summary>
        /// Пол (true - мужской)
        /// </summary>
        public bool? IsMale { get; set; }

        /// <summary>
        /// Первая буква наименования
        /// </summary>
        public char FirstChar { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// по краткому имени равенство
        /// </summary>
        public string NameExact { get; set; }

        /// <summary>
        /// по имени равенство
        /// </summary>
        public string FirstNameExact { get; set; }

        /// <summary>
        /// по фамилии равенство
        /// </summary>
        public string LastNameExact { get; set; }

        /// <summary>
        /// по серии паспорта равенство
        /// </summary>
        public string PassportSerialExact { get; set; }

        /// <summary>
        /// по номеру паспорта равенство
        /// </summary>
        public int? PassportNumberExact { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? BirthDateExact { get { return _BirthDateExact; } set { _BirthDateExact = value.ToUTC(); } }
        private DateTime? _BirthDateExact;
    }
}
