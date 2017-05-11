using System;
using BL.Model.Extensions;

namespace BL.Model.WebAPI.FrontModel
{
    public class FrontSystemSession
    {
        //public string Token { get; set; }

        public DateTime? LastUsage { get { return _LastUsage; } set { _LastUsage = value.ToUTC(); } }
        private DateTime? _LastUsage;

        public DateTime CreateDate { get { return _CreateDate; } set { _CreateDate = value.ToUTC(); } }
        private DateTime _CreateDate;

        public int? LoginLogId { get; set; }
        public string LoginLogInfo { get; set; }
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string LogException { get; set; }
        /// <summary>
        /// Тип ДМС ошибки
        /// </summary>
        public string TypeException { get; set; }
        /// <summary>
        /// Объект к логу
        /// </summary>
        public string ObjectLog { get; set; }
        /// <summary>
        /// ИД веб пользователя
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// ИД сотрудника
        /// </summary>
        public int? AgentId { get; set; }
        /// <summary>
        /// Тип лога
        /// </summary>
        public int? LogLevel { get; set; }
        /// <summary>
        /// ФИО
        /// </summary>
        public string Name { get; set; }
        public int ClientId { get; set; }
        /// <summary>
        /// Признак успешности входа
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// Активная сессия
        /// </summary>
        public bool IsActive { get; set; }

    }
}
