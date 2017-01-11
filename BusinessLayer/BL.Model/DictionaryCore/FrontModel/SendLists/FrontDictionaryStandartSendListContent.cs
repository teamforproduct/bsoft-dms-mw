using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Содержание типового списка рассылки
    /// </summary>
    public class FrontDictionaryStandartSendListContent
    {
        /// <summary>
        /// ИД
        /// </summary>
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
        public int SendTypeId { get; set; }
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
        private DateTime?  _DueDate; 
        /// <summary>
        /// Срок исполнения (кол-во дней)
        /// </summary>
        public int? DueDay { get; set; }
        /// <summary>
        /// Уровень доступа (ссылка)
        /// </summary>
        public int? AccessLevelId { get; set; }
        /// <summary>
        /// Название типа рассылки
        /// </summary>
        public string SendTypeName { get; set; }
        /// <summary>
        /// Название должности получателя
        /// </summary>
        public string TargetPositionName { get; set; }
        /// <summary>
        /// ФИО получателя
        /// </summary>
        public string TargetAgentName { get; set; }
        /// <summary>
        /// Название уровня доступа
        /// </summary>
        public string AccessLevelName { get; set; }
        /// <summary>
        /// Тип рассылки
        /// </summary>
        public bool SendTypeIsExternal { get; set; }

    }
}
