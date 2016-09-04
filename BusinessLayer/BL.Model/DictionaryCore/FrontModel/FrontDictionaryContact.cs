using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контакт агента
    /// </summary>
    public class FrontDictionaryContact
    {
        /// <summary>
        /// ИД контакта
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Агент (сотрудник, юр.лицо, банк, физ.лицо)
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// Тип контакта
        /// </summary>
        public FrontDictionaryContactType ContactType { get; set; }
        
        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Признак подтверждения
        /// </summary>
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// Признак основного контакта
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }
    }
}
