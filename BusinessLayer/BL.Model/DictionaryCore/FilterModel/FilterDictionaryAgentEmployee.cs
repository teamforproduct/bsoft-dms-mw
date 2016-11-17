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
        /// Строка, для полнотекстового поиска
        /// </summary>
        public string FullTextSearchString { get; set; }

        /// <summary>
        /// по имени (по равенству)
        /// </summary>
        public string NameExact { get; set; }

        /// <summary>
        /// табельный номер
        /// </summary>
        public int? PersonnelNumber { get; set; }

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

        public string FullName { get; set; }

        public string FirstNameExact { get; set; }
        public string LastNameExact { get; set; }
        public string PassportSerialExact { get; set; }
        public int? PassportNumberExact { get; set; }
        public DateTime? BirthDateExact { get; set; }

        /// <summary>
        /// Сотрудники, которым назначены указанные роли
        /// </summary>
        public List<int> RoleIDs { get; set; }
    }
}
