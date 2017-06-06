using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using BL.Model.DocumentCore.Actions;
using BL.Model.Enums;
using BL.Model.Users;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/изменения записи плана работы над документом
    /// </summary>
    public class BaseModifyDocumentSendList
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Номер этапа, если null, то выполняется ПРЯМОЕ действие без сохранения в план
        /// </summary>
        public int? Stage { get; set; }
        /// <summary>
        /// ИД типа этапа
        /// </summary>
        public EnumStageTypes? StageType { get; set; }
        /// <summary>
        /// ИД типа рассылки
        /// </summary>
        [Required]
        public EnumSendTypes SendType { get; set; }
        /// <summary>
        /// ИД должности кому направлена рассылка
        /// </summary>
        public int? TargetPositionId { get; set; }
        /// <summary>
        /// ИД Задачи
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Добавлять самоконтроль для отправителя
        /// </summary>
        public bool IsAddControl { get; set; }
        /// <summary>
        /// Комментарий к самоконтролю
        /// </summary>
        public string SelfDescription { get; set; }
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
        /// </summary>
        public Nullable<DateTime> SelfAttentionDate { get; set; }
        /// <summary>
        /// Дни для определения даты постоянного внимания для самоконтроля
        /// </summary>
        public int? SelfAttentionDay { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Срок исполнения (дата)
        /// </summary>
        public DateTime? DueDate { get { return _DueDate; } set { _DueDate = value.ToUTC(); } }
        private DateTime? _DueDate;
        /// <summary>
        /// Срок исполнения (дни)
        /// </summary>
        public int? DueDay { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        [Required]
        public EnumAccessLevels AccessLevel { get; set; }
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
        /// <summary>
        /// группы получателей
        /// </summary>
        public List<AccessGroup> TargetAccessGroups { get; set; }
    }
}
