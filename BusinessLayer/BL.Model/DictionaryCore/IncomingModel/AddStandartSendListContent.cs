using BL.Model.Enums;
using BL.Model.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Содержание типового списка рассылки
    /// </summary>
    public class AddStandartSendListContent
    {
        /// <summary>
        /// Ссылка на типовую рассылку
        /// </summary>
        [Required]
        public int StandartSendListId { get; set; }
        /// <summary>
        /// Этап
        /// </summary>
        public int Stage { get; set; }
        /// <summary>
        /// Ссылка на тип рассылки
        /// </summary>
        public EnumSendTypes SendTypeId { get; set; }
        /// <summary>
        /// Должность получателя
        /// </summary>
        public int? TargetPositionId { get; set; }
        /// <summary>
        /// ПОльзователь-получатель
        /// </summary>
        public int? TargetAgentId { get; set; }
        /// <summary>
        /// Задача
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Комментарии
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Исполнить до (дата)
        /// </summary>
        public DateTime? DueDate { get { return _DueDate; } set { _DueDate=value.ToUTC(); } }
        private DateTime? _DueDate;
		
        /// <summary>
        /// Срок исполнения (кол-во дней)
        /// </summary>
        public int? DueDay { get; set; }
        /// <summary>
        /// Уровень доступа (ссылка)
        /// </summary>
        public EnumAccessLevels? AccessLevelId { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
    }
}
