using BL.CrossCutting.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using BL.Model.Users;
using BL.Model.Extensions;

namespace BL.CrossCutting.Context
{

    public class FrontSystemSession
    {
        public string Token { get; set; }

        public DateTime? LastUsage { get { return _LastUsage; } set { _LastUsage = value.ToUTC(); } }
        private DateTime? _LastUsage;

        public DateTime CreateDate { get { return _CreateDate; } set { _CreateDate = value.ToUTC(); } }
        private DateTime _CreateDate;

        public int? LoginLogId { get; set; }
        public string LoginLogInfo { get; set; }
        /// <summary>
        /// ИД веб пользователя
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// ИД сотрудника
        /// </summary>
        public int? AgentId { get; set; }
        /// <summary>
        /// ФИО
        /// </summary>
        public string Name { get; set; }
        public int ClientId { get; set; }
        /// <summary>
        /// Активная сессия
        /// </summary>
        public bool IsActive { get; set; }

    }
}
