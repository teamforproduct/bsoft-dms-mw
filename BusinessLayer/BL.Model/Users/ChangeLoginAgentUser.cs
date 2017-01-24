using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class ChangeLoginAgentUser: ChangeLogin
    {
        /// <summary>
        /// Id сотрудника - пользователя
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// EmailConfirmRequired
        /// </summary>
        public bool IsVerificationRequired { get; set; }

    }
}
