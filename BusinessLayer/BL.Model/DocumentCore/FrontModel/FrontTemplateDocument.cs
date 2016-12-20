using System;
using System.Collections.Generic;
using BL.Model.Enums;
using BL.Model.SystemCore.FrontModel;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Модель шаблона документа для фронта
    /// </summary>
    public class FrontTemplateDocument
    {
        /// <summary>
        /// ИД Шаблона
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя шаблона - обязательное
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ИД Направление документа - обязательное
        /// </summary>
        public EnumDocumentDirections DocumentDirection { get; set; }
        /// <summary>
        /// Название гаправления
        /// </summary>
        public string DocumentDirectionName { get; set; }
        /// <summary>
        /// ИД Тип документа - обязательное
        /// </summary>
        public int DocumentTypeId { get; set; }
        /// <summary>
        /// Название типа документа
        /// </summary>
        public string DocumentTypeName { get; set; }
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
        /// Признак того, что шаблон может использоваться
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// ИД Тематика документа
        /// </summary>
        public int? DocumentSubjectId { get; set; }
        /// <summary>
        /// Название тематики документа
        /// </summary>
        public string DocumentSubjectName { get; set; }
        /// <summary>
        /// Описания документа
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ИД журнала регистрации
        /// </summary>
        public int? RegistrationJournalId { get; set; }
        /// <summary>
        /// Название журнала регистрации
        /// </summary>
        public string RegistrationJournalName { get; set; }
        /// <summary>
        /// ИД Контрагент, от которого получен документ - только для входящих
        /// </summary>
        public int? SenderAgentId { get; set; }
        /// <summary>
        /// Название Контрагента, от которого получен документ - только для входящих
        /// </summary>
        public string SenderAgentName { get; set; }
        /// <summary>
        /// ИД Контактное лицо в организации
        /// </summary>
        public int? SenderAgentPersonId { get; set; }
        /// <summary>
        /// Название Контактное лицо в организации
        /// </summary>
        public string SenderAgentPersonName { get; set; }
        /// <summary>
        /// Кому адресован документ - только для входящих
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// Есть ли документы, созданные по шаблону
        /// </summary>
        public bool? IsUsedInDocument { get; set; }
        /// <summary>
        /// Динамические аттрибуты шаблона документа
        /// </summary>
        public IEnumerable<FrontPropertyValue> Properties { get; set; }

    }
}
