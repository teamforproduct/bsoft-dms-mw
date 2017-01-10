using System;
using BL.Model.Enums;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontTemplateDocumentSendList
    {
        /// <summary>
        /// ИД записи плана
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ИД шаблона
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД типа рассылки
        /// </summary>
        public int? SendType { get; set; }
        /// <summary>
        /// Название типа рассылки
        /// </summary>
        public string SendTypeName { get; set; }
        /// <summary>
        /// ИД типа этапа
        /// </summary>
        public int? StageType { get; set; }
        /// <summary>
        /// Название типа этапа
        /// </summary>
        public string StageTypeName { get; set; }
        /// <summary>
        /// ИД одтела должности, кому направлена рассылка
        /// </summary>
        public int? TargetPositionDepartmentId { get; set; }
        /// <summary>
        /// Название отдела должности, кому направлена рассылка
        /// </summary>
        public string TargetPositionDepartmentName { get; set; }
        /// <summary>
        /// ИД должности кому направлена рассылка
        /// </summary>
        public int? TargetPositionId { get; set; }
        /// <summary>
        /// Название должности кому направлена рассылка
        /// </summary>
        public string TargetPositionName { get; set; }
        /// <summary>
        /// Имя кому направлена рассылка
        /// </summary>
        public string TargetPositionExecutorAgentName { get; set; }
        /// <summary>
        /// ИД внешнего агента кому направлена рассылка
        /// </summary>
        public int? TargetAgentId { get; set; }
        /// <summary>
        /// Название внешнего агента кому направлена рассылка
        /// </summary>
        public string TargetAgentName { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Номер этапа
        /// </summary>
        public int Stage { get; set; }
        /// <summary>
        /// ИД Задачи
        /// </summary>
        public int? TaskId { get; set; }
        /// <summary>
        /// Название задачи
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// Срок исполнения (дни)
        /// </summary>
        public int? DueDay { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        public int? AccessLevelId { get; set; }
        /// <summary>
        /// Название уровня доступа
        /// </summary>
        public string AccessLevelName { get; set; }
        /// <summary>
        /// Признак "В рамках рабочей группы"
        /// </summary>
        public bool? IsWorkGroup { get; set; }
        /// <summary>
        /// Добавлять самоконтроль для отправителя
        /// </summary>
        public bool? IsAddControl { get; set; }
        /// <summary>
        /// Комментарий к самоконтролю
        /// </summary>
        public string SelfDescription { get; set; }
        /// <summary>
        /// Срок для самоконтроля (дни)
        /// </summary>
        public int? SelfDueDay { get; set; }
        /// <summary>
        /// Дни для определения даты постоянного внимания для самоконтроля
        /// </sumпmary>
        public int? SelfAttentionDay { get; set; }
        /// <summary>
        /// Событие доступно в рамках задачи
        /// </summary>
        public bool? IsAvailableWithinTask { get; set; }

    }
}
