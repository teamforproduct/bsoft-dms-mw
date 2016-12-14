using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контакт агента
    /// </summary>
    // КАПеЦ!!!!!! Эта модель полная копия AddDictionaryContact. отличается только атрибутом для AgentId
    public class AddDictionaryContactPersonContact 
    {
        /// <summary>
        /// Агент (сотрудник, юр.лицо, банк, физ.лицо, компания)
        /// </summary>
        [IgnoreDataMember]
        public int AgentId { get; set; }

        /// <summary>
        /// Тип контакта
        /// </summary>
        [Required]
        public int ContactTypeId { get; set; }

        /// <summary>
        /// Контакты
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// Дополнительная информация. Сюда будут записывать контакт, если маска не пропустит :)
        /// </summary>
        public string Description { get; set; }

    }
}
