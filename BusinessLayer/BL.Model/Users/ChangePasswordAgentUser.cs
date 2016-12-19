using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class ChangePasswordAgentUser : ChangePassword
    {
        [Required]
        public int AgentId { get; set; }

        /// <summary>
        /// Требуется изменение пароля при следующем входе в систему
        /// </summary>
        public bool IsChangePasswordRequired { get; set; }

        /// <summary>
        /// Немедленно убить все текущие сессии пользователя
        /// </summary>
        public bool IsKillSessions { get; set; }

    }
}
