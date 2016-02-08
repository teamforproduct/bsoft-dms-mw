using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    public class RegisterDocument
    {
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД Журнала регистрации
        /// </summary>
        public int RegistrationJournalId { get; set; }
        /// <summary>
        /// Если 0 - регистрируется документ
        /// Если 1 - автоматически получается и резервируется новый регистрационный номер
        /// </summary>
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
        public DateTime RegistrationDate { get; set; }    
    }
}
