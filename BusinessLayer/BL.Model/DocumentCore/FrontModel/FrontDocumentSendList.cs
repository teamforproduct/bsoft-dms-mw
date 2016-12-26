using System;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using BL.Model.DocumentCore.Actions;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentSendList
    {
        /// <summary>
        /// ИЗ записи плана
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Номер этапа
        /// </summary>
        public int Stage { get; set; }
        /// <summary>
        /// ИД типа этапа
        /// </summary>
        public EnumStageTypes? StageType { get; set; }
        /// <summary>
        /// ИД типа рассылки
        /// </summary>
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
        /// </sumпmary>
        public Nullable<DateTime> SelfAttentionDate { get; set; }
        /// <summary>
        /// Дни для определения даты постоянного внимания для самоконтроля
        /// </sumпmary>
        public int? SelfAttentionDay { get; set; }
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
        public DateTime? DueDate { get { return _DueDate; } set { _DueDate = value.ToUTC(); } }
        private DateTime? _DueDate;
        /// <summary>
        /// Срок исполнения (дни)
        /// </summary>
        public int? DueDay { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
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

        public int? StartEventId { get; set; }
        public int? CloseEventId { get; set; }
        public string StageTypeName { get; set; }
        public string StageTypeCode { get; set; }
        public string SendTypeName { get; set; }
        public string SendTypeCode { get; set; }
        public bool SendTypeIsImportant { get; set; }
        public int SourceAgentId { get; set; }
        public string SourcePositionName { get; set; }
        public int? SourcePositionExecutorAgentId { get; set; }
        public string SourcePositionExecutorAgentName { get; set; }
        public string SourceAgentName { get; set; }

        public string TargetPositionName { get; set; }
        public int? TargetPositionExecutorAgentId { get; set; }
        public string TargetPositionExecutorAgentName { get; set; }
        public string TargetAgentName { get; set; }

        public string SourcePositionExecutorNowAgentName { get; set; }
        public string SourcePositionExecutorAgentPhoneNumber { get; set; }

        public string TargetPositionExecutorNowAgentName { get; set; }
        public string TargetPositionExecutorAgentPhoneNumber { get; set; }

        public string AccessLevelName { get; set; }

        public string AddDescription { get; set; }

        public FrontDocumentEvent StartEvent { get; set; }
        public FrontDocumentEvent CloseEvent { get; set; }
    }
}
