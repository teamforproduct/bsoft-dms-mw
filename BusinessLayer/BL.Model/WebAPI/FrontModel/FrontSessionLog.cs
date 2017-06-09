using System;
using BL.Model.Extensions;

namespace BL.Model.WebAPI.FrontModel
{
    public class FrontSessionLog
    {
        public int Id { get; set; }

        public DateTime? LastUsage { get { return _LastUsage; } set { _LastUsage = value.ToUTC(); } }
        private DateTime? _LastUsage;

        public DateTime Date { get { return _Date; } set { _Date = value.ToUTC(); } }
        private DateTime _Date;

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public string Type { get; set; }

        public string Event { get; set; }

        /// <summary>
        /// ИД веб пользователя
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// ФИО
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Признак успешности входа
        /// </summary>
        public bool IsSuccess { get; set; } = true;
        /// <summary>
        /// Активная сессия
        /// </summary>
        public bool IsActive { get; set; }

        public string Host { get; set; }

        public string IP { get; set; }

        public string Platform { get; set; }

        public string Browser { get; set; }

        public string Fingerprint { get; set; }

    }
}

