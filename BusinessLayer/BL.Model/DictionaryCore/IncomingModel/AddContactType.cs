using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Типы контактов
    /// </summary>
    public class AddContactType
    {

        /// <summary>
        /// Название типа контакта
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Маска ввода
        /// </summary>
        public string InputMask { get; set; }

        /// <summary>
        /// Краткий идентификатор
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// Краткий идентификатор
        /// </summary>
        [IgnoreDataMember]
        public string SpecCode { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}
