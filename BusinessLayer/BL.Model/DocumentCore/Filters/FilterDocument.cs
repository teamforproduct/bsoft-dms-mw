using System;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтра документов
    /// </summary>
    public class FilterDocument
    {
        /// <summary>
        /// Массив ИД документов
        /// </summary>
        public List<int> Id { get; set; }
        /// <summary>
        /// Массив ИД шаблонов документов
        /// </summary>
        public List<int> TemplateDocumentId { get; set; }
        /// <summary>
        /// Массив ИД направлений документов
        /// </summary>
        public List<int> DocumentDirectionId { get; set; }
        /// <summary>
        /// Массив ИД типов документов
        /// </summary>
        public List<int> DocumentTypeId { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате создания
        /// </summary>
        public DateTime? CreateFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате создания
        /// </summary>
        public DateTime? CreateToDate { get; set; }
        /// <summary>
        /// Массив ИД тематик документов
        /// </summary>
        public List<int> DocumentSubjectId { get; set; }
        /// <summary>
        /// Отрывок краткого содержания
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Признак регистрации документа
        /// </summary>
        public bool? IsRegistered { get; set; }
        /// <summary>
        /// Массив ИД журналов регистрации документов
        /// </summary>
        public List<int> RegistrationJournalId { get; set; }
        /// <summary>
        /// Отрывок полного регистрационного номера
        /// </summary>
        public string RegistrationNumber { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате регистрации документа
        /// </summary>
        public DateTime? RegistrationFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате регистрации документа
        /// </summary>
        public DateTime? RegistrationToDate { get; set; }
        /// <summary>
        /// Массив ИД ответственного по документу
        /// </summary>
        public List<int> ExecutorPositionId { get; set; }
        /// <summary>
        /// Массив ИД организаций
        /// </summary>
        public List<int> SenderAgentId { get; set; }
        /// <summary>
        /// Массив ИД контактов
        /// </summary>
        public List<int> SenderAgentPersonId { get; set; }
        /// <summary>
        /// Отрывок входящего номера документа
        /// </summary>
        public string SenderNumber { get; set; }
        /// <summary>
        /// Дата "с" для отбора по дате входящего документа
        /// </summary>
        public DateTime? SenderFromDate { get; set; }
        /// <summary>
        /// Дата "по" для отбора по дате входящего документа
        /// </summary>
        public DateTime? SenderToDate { get; set; }
        /// <summary>
        /// Отрывок кому адресован документ
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// Признак в работе
        /// </summary>
        public bool IsInWork { get; set; } // should be true by default
    }
}
