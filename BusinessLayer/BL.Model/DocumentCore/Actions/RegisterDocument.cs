using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для регистрации документа
    /// </summary>
    public class RegisterDocument
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
        /// Если 0 - регистрируется документ
        /// Если 1 - автоматически получается и резервируется новый регистрационный номер
        /// </summary>
        [Required]
        public bool IsOnlyGetNextNumber { get; set; }
        /// <summary>
        /// Регистрационный номер.
        /// Если не передается, автоматически получается и резервируется новый регистрационный номер
        /// </summary>
        public int? RegistrationNumber { get; set; }
        /// <summary>
        /// Суффикс регистрационного номера. При автоматическом получении номера заполняется из параметров журнала регистрации.
        /// </summary>
        public string RegistrationNumberSuffix { get; set; }
        /// <summary>
        /// Префикс регистрационного номера. При автоматическом получении номера заполняется из параметров журнала регистрации.
        /// </summary>
        public string RegistrationNumberPrefix { get; set; }
        /// <summary>
        /// Дата регистрации документа
        /// </summary>
        [Required]
        public DateTime RegistrationDate { get; set; }
        /// <summary>
        /// Не запонять!!!
        /// Префикс для нумерации
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public string NumerationPrefixFormula { get; set; }
    }
}
