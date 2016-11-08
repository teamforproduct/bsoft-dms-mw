using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class ChangeLockoutAgentUser
    {
        [Required]
        public int AgentId { get; set; }

        /// <summary>
        /// блокирование/разблокирование пользователя
        /// </summary>
        public bool IsLockout { get; set; }

        /// <summary>
        /// Немедленно убить все текущие сессии пользователя
        /// </summary>
        public bool IsKillSessions { get; set; }
    }
}
