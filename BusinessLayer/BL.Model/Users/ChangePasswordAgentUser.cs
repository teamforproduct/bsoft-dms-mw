﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class ChangePasswordAgentUser
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

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
