using BL.Model.Common;
using BL.Model.Extensions;
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
        //public DateTime? BirthDate { get { return _BirthDate; } set { _BirthDate = value.ToUTC(); } }
        //private DateTime? _BirthDate;

        public Period BirthPeriod { get; set; }

        /// <summary>
        /// Пол (true - мужской)
        /// </summary>
        public bool? IsMale { get; set; }

        public string FullName { get; set; }

        public string FirstNameExact { get; set; }
        public string LastNameExact { get; set; }
        public string PassportSerialExact { get; set; }
        public int? PassportNumberExact { get; set; }
        public DateTime? BirthDateExact { get { return _BirthDateExact; } set { _BirthDateExact = value.ToUTC(); } }
        private DateTime? _BirthDateExact;

        /// <summary>
        /// Сотрудники, которым назначены указанные роли
        /// </summary>
        public List<int> RoleIDs { get; set; }

        public List<int> AddressIDs { get; set; }

        public List<int> ContactIDs { get; set; }

        public List<int> PositionIDs { get; set; }
    }
}
