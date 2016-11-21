using BL.CrossCutting.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using BL.Model.Users;

namespace BL.CrossCutting.Context
{

    public class FrontSystemSession
    {
        public string Token { get; set; }
        public DateTime? LastUsage { get; set; }
        public DateTime CreateDate { get; set; }
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
