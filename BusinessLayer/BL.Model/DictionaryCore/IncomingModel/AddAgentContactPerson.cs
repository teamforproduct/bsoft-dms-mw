using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddAgentContactPerson : AddAgent
    {
        /// <summary>
        /// Компания юридическое лицо
        /// </summary>
        [Required]
        public int CompanyId { get; set; }

        /// <summary>
        /// Имя (для отчетов)
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия (для отчетов)
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Отчество (для отчетов)
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        public bool IsMale { get; set; }

        /// <summary>
        /// Должность (текстовое поле)
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Активность
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

    }
}
