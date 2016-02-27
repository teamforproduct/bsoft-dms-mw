using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using BL.Model.Enums;
using BL.Model.Users;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/изменения записи плана работы над документом
    /// </summary>
    public class ModifyDocumentSendList: CurrentPosition
    {
        /// <summary>
        /// ИЗ записи плана
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Номер этапа
        /// </summary>
        [Required]
        public int Stage { get; set; }
        /// <summary>
        /// ИД типа рассылки
        /// </summary>
        [Required]
        public EnumSendTypes SendType { get; set; }
        /// <summary>
        /// ИД должности от кого идет рассылка
        /// </summary>
        public int? SourcePositionId { get; set; }
        /// <summary>
        /// ИД должности кому направлена рассылка
        /// </summary>
        public int? TargetPositionId { get; set; }
        /// <summary>
        /// ИД внешнего агента кому направлена рассылка
        /// </summary>
        public int? TargetAgentId { get; set; }
        /// <summary>
        /// Задача
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Срок исполнения (дата)
        /// </summary>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Срок исполнения (дни)
        /// </summary>
        public int? DueDay { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        [Required]
        public EnumDocumentAccesses AccessLevel { get; set; }
        /// <summary>
        /// Не запонять!!!
        /// Признак первоначальный пункт
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public bool IsInitial { get; set; }
    }
}
