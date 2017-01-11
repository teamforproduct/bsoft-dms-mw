using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.SystemCore.IncomingModel;
using System.ComponentModel.DataAnnotations;
using BL.Model.SystemCore.FrontModel;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class AddTemplateDocument
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
        public EnumDocumentDirections DocumentDirection { get; set; }
        /// <summary>
        /// ИД Тип документа - обязательное
        /// </summary>
        [Required]
        public int DocumentTypeId { get; set; }
        /// <summary>
        /// ИД Тематика документа
        /// </summary>
        public int? DocumentSubjectId { get; set; }
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
