using BL.Model.Enums;
using BL.Model.SystemCore.FrontModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class AddTemplate
    {
        /// <summary>
        /// Имя шаблона - обязательное
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Признак, можно ли указанные в шаблоне значения поменять в документе
        /// </summary>
        public bool IsHard { get; set; }
        /// <summary>
        /// Признак того, что шаблон может использоваться для создания проектов
        /// </summary>
        public bool IsForProject { get; set; }
        /// <summary>
        /// Признак того, что шаблон может использоваться для создания документов
        /// </summary>
        public bool IsForDocument { get; set; }

        /// <summary>
        /// ИД Направление документа - обязательное
        /// </summary>
        [Required]
        public EnumDocumentDirections DocumentDirection { get; set; }
        /// <summary>
        /// ИД Тип документа - обязательное
        /// </summary>
        [Required]
        public int DocumentTypeId { get; set; }
        /// <summary>
        /// Тематика документа
        /// </summary>
        public string DocumentSubject { get; set; }
        /// <summary>
        /// Описания документа
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ИД журнала регистрации
        /// </summary>
        public int? RegistrationJournalId { get; set; }
        /// <summary>
        /// ИД Контрагент, от которого получен документ - только для входящих
        /// </summary>
        public int? SenderAgentId { get; set; }
        /// <summary>
        /// ИД Контактное лицо в организации
        /// </summary>
        public int? SenderAgentPersonId { get; set; }
        /// <summary>
        /// Кому адресован документ - только для входящих
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// Признак того, что шаблон может использоваться
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Динамические аттрибуты шаблона документа
        /// </summary>
        public IEnumerable<FrontPropertyValue> Properties { get; set; }
    }
}
