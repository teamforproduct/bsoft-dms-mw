using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontTemplateSendList
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
        public EnumSendTypes? SendType { get; set; }
        /// <summary>
        /// Название типа рассылки
        /// </summary>
        public string SendTypeName { get; set; }
        /// <summary>
        /// ИД типа этапа
        /// </summary>
        public EnumStageTypes? StageType { get; set; }
        /// <summary>
        /// Название типа этапа
        /// </summary>
        public string StageTypeName { get; set; }
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
        public EnumAccessLevels? AccessLevel { get; set; }
        /// <summary>
        /// Название уровня доступа
        /// </summary>
        public string AccessLevelName { get; set; }
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
        /// </summary>
        public int? SelfAttentionDay { get; set; }
        /// <summary>
        /// Группы получателей
        /// </summary>
        public IEnumerable<FrontTemplateSendListAccessGroup> AccessGroups { get; set; }

    }
}
