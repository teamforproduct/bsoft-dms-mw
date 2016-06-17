using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using BL.Model.DocumentCore.Actions;
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
        /// В рамках рабочей группы
        /// </summary>
        public bool IsWorkGroup { get; set; }
        /// <summary>
        /// Добавлять самоконтроль для отправителя
        /// </summary>
        public bool IsAddControl { get; set; }
        /// <summary>
        /// Срок для самоконтроля (дата)
        /// </summary>
        public Nullable<DateTime> SelfDueDate { get; set; }
        /// <summary>
        /// Срок для самоконтроля (дни)
        /// </summary>
        public int? SelfDueDay { get; set; }
        /// <summary>
        /// Дата постоянное внимание для самоконтроля
        /// </sumпmary>
        public Nullable<DateTime> SelfAttentionDate { get; set; }
        /// <summary>
        /// Событие доступно в рамках задачи
        /// </summary>
        public bool IsAvailableWithinTask { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
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
        /// Признак первоначальный пункт
        /// </summary>
        public bool IsInitial { get; set; }
        /// <summary>
        /// Массив событий, по перемещению бумажных насителей
        /// </summary>
        public List<PaperEvent> PaperEvents { get; set; }
        /// <summary>
        /// Признак, запуска пункта плана сразу после сохранения
        /// </summary>
        public bool? IsLaunchItem { get; set; }
    }
}
