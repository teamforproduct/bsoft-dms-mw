using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using BL.Model.Enums;
using BL.Model.Extensions;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Содержание типового списка рассылки
    /// </summary>
    public class ModifyDictionaryStandartSendListContent
    {
        /// <summary>
        /// ИД
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Ссылка на типовую рассылку
        /// </summary>
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
        public EnumDocumentAccesses? AccessLevelId { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
    }
}
