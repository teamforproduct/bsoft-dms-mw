using System;
using BL.Model.Enums;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore
{
    /// <summary>
    /// Модель для добавления/изменения записи плана работы над документом
    /// </summary>
    public class ModifyDocumentSendList
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
        /// ИД должности кому направлена рассылка
        /// </summary>
        [Required]
        public Nullable<int> TargetPositionId { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Срок исполнения (дата)
        /// </summary>
        public Nullable<DateTime> DueDate { get; set; }
        /// <summary>
        /// Срок исполнения (дни)
        /// </summary>
        public int? DueDay { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        [Required]
        public EnumDocumentAccesses AccessLevel { get; set; }
    }
}
