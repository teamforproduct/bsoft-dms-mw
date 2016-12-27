using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using BL.Model.Extensions;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Человек
    /// </summary>
    public class AddAgentPeople : AddAgent
    {
        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
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
        /// Дата рождения
        /// </summary>
        public DateTime? BirthDate { get { return _BirthDate; } set { _BirthDate=value.ToUTC(); } }
        private DateTime? _BirthDate;

    }
}
