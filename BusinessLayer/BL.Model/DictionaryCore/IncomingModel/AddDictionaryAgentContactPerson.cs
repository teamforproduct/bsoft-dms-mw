using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class AddDictionaryAgentContactPerson
    {
        /// <summary>
        /// Компания юридическое лицо
        /// </summary>
        [Required]
        public int CompanyId { get; set; }

        /// <summary>
        /// Краткое имя
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Контакты
        /// </summary>
        public IEnumerable<AddDictionaryContactPersonContact> Contacts { get; set; }

    }
}
