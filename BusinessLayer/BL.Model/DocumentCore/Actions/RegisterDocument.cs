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
    public class RegisterDocument : RegisterDocumentBase
    {
        /// <summary>
        /// Признак регистрировать ли документ
        /// </summary>
        [Required]
        public bool IsRegistered { get; set; }
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
        /// Не запонять!!!
        /// Префикс для нумерации
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public string NumerationPrefixFormula { get; set; }
    }
}
