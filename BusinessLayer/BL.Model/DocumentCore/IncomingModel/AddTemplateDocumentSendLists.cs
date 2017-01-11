using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;
using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/изменения записи плана работы для шаблона
    /// </summary>
    public class AddTemplateDocumentSendLists 
    {
        /// <summary>
        /// ИД шаблона
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД типа рассылки
        /// </summary>
        public EnumSendTypes SendType { get; set; }
        /// <summary>
        /// ИД должности кому направлена рассылка
        /// </summary>
        public int? TargetPositionId { get; set; }
        /// <summary>
        /// ИД внешнего агента кому направлена рассылка
        /// </summary>
        public int? TargetAgentId { get; set; }
        /// <summary>
        /// ИД Задачи
        /// </summary>
        public int? TaskId { get; set; }
        /// <summary>
        /// Признак "В рамках рабочей группы"
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
        /// Срок для самоконтроля (дни)
        /// </summary>
        public int? SelfDueDay { get; set; }
        /// <summary>
        /// Дни для определения даты постоянного внимания для самоконтроля
        /// </summary>
        public int SelfAttentionDay { get; set; }
        /// <summary>
        /// Событие доступно в рамках задачи
        /// </summary>
        public bool IsAvailableWithinTask { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Номер этапа
        /// </summary>
        public int Stage { get; set; }
        /// <summary>
        /// ИД типа этапа
        /// </summary>
        public EnumStageTypes? StageType { get; set; }
        /// <summary>
        /// Срок исполнения (дни)
        /// </summary>
        public int? DueDay { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        public EnumDocumentAccesses AccessLevel { get; set; }
    }
}
