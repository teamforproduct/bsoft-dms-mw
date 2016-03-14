using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class FrontDictionaryAgentPerson : FrontDictionaryAgent
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }
        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// Пол (true - мужской)
        /// </summary>
        public bool IsMale { get; set; }
        /// <summary>
        /// Серия паспорта
        /// </summary>
        public string PassportSerial { get; set; }
        /// <summary>
        /// Номер паспорта
        /// </summary>
        public int? PassportNumber { get; set; }
        /// <summary>
        /// Дата выдачи паспорта
        /// </summary>
        public DateTime? PassportDate { get; set; }
        /// <summary>
        /// Кем выдан паспорт
        /// </summary>
        public string PassportText { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public override string Description { get; set; }
        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get
                {
                return LastName.Trim() + " " + FirstName.Trim() + " " + MiddleName.Trim();
                }
                               }
        /// <summary>
        /// Сокращенное имя
        /// </summary>
        public string ShortName { get
            {
                return LastName.Trim() + " " +FirstName.Trim().Substring(1, 1) + "." + MiddleName.Trim().Substring(1, 1) + ".";
            }
                }
        /// <summary>
        /// Паспортные данные
        /// </summary>
        public string Passport  { get
            {
                return PassportSerial.Trim() + "-" + PassportNumber.ToString() + " выдан " + PassportText.Trim() + " " + PassportDate?.Date;
            }

        }

        public override bool IsCompany {
            get  {
                return false;
            }
        }

        public override bool IsBank
        {
            get  {
                return false;
            }
        }
        public override bool IsEmployee
        {
            get
            {
                return false;
            }
        }
        public override bool IsIndividual
        {
            get
            {
                return true;
            }
        }
    }
}
