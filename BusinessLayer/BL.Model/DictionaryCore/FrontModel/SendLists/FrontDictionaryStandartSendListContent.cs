using BL.Model.Extensions;
using System;
using System.Runtime.Serialization;

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
        /// Получатель. Название должности
        /// </summary>
        public string TargetPositionName { get; set; }

        /// <summary>
        /// Получатель. ФИО
        /// </summary>
        public string TargetExecutorName { get; set; }

        /// <summary>
        /// Получатель. Аватарка
        /// </summary>
        public string TargetExecutorImage { get { return Converter.ToBase64String(imageByteArray); } }

        [IgnoreDataMember]
        public byte[] ImageByteArray { set { imageByteArray = value; } }
        private byte[] imageByteArray;

        /// <summary>
        /// Получатель. Суффикс типа исполнения
        /// </summary>
        public string TargetExecutorTypeSuffix { get; set; }

        /// <summary>
        /// Получатель. Индекс отдела
        /// </summary>
        public string TargetDepartmentIndex { get; set; }

        /// <summary>
        /// Получатель. Наименование отдела
        /// </summary>
        public string TargetDepartmentName { get; set; }
        
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
