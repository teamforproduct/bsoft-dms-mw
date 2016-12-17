using BL.Model.DictionaryCore.FrontModel;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontMainModel
{
    /// <summary>
    /// Контрагент - сотрудник
    /// </summary>
    public class FrontMainDictionaryAgentEmployee
    {

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// ФИО, Название, ...
        /// </summary>
        public string Name { get; set; }



        /// <summary>
        /// Аватарка
        /// </summary>
        public string Image { get { return Converter.ToBase64String(imageByteArray); } }

        [IgnoreDataMember]
        public byte[] ImageByteArray { set { imageByteArray = value; } }
        private byte[] imageByteArray;


        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get { return _BirthDate; } set { _BirthDate = value.ToUTC(); } }
        private DateTime? _BirthDate;


        /// <summary>
        /// Серия паспорта
        /// </summary>
        [IgnoreDataMember]
        public string PassportSerial { get; set; }
        /// <summary>
        /// Номер паспорта
        /// </summary>
        [IgnoreDataMember]
        public int? PassportNumber { get; set; }
        /// <summary>
        /// Дата выдачи паспорта
        /// </summary>
        [IgnoreDataMember]
        public DateTime? PassportDate { get { return _PassportDate; } set { _PassportDate = value.ToUTC(); } }
        private DateTime? _PassportDate;
        /// <summary>
        /// Кем выдан паспорт
        /// </summary>
        [IgnoreDataMember]
        public string PassportText { get; set; }

        /// <summary>
        /// Паспортные данные
        /// </summary>
        public string Passport
        {
            get { string pass = PassportSerial?.Trim() + " " + PassportNumber?.ToString() + " " + PassportText?.Trim() + " " + PassportDate?.ToString("dd.MM.yyyy"); return pass.Trim(); }
        }

        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxCode { get; set; }


        /// <summary>
        /// табельный номер сотрудника
        /// </summary>
        public int PersonnelNumber { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// Признак активности
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Список адресов контрагента
        /// </summary>
        public IEnumerable<FrontDictionaryPositionExecutor> PositionExecutors { get; set; }

    }
}
