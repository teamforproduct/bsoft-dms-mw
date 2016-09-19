using BL.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр сотрудников
    /// </summary>
    public class FilterDictionaryAgentEmployee : DictionaryBaseFilterParameters
    {
        /// <summary>
        /// по имени (по равенству)
        /// </summary>
        public string NameExact { get; set; }

        /// <summary>
        /// табельный номер
        /// </summary>
        public string PersonnelNumber { get; set; }
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

        public string FirstNameExact { get; set; }
        public string LastNameExact { get; set; }
        public string PassportSerial { get; set; }
        public int? PassportNumber { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
