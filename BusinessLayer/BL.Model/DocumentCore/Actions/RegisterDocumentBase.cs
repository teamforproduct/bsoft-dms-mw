using BL.Model.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для регистрации документа
    /// </summary>
    public class RegisterDocumentBase : CurrentPosition
    {
        /// <summary>
        /// ИД Документа
        /// </summary>
        [Required]      
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД Журнала регистрации
        /// </summary>
        [Required]
        public int RegistrationJournalId { get; set; }
        /// <summary>
        /// Дата регистрации документа
        /// </summary>
        [Required]
        public DateTime RegistrationDate { get; set; }
    }
}
