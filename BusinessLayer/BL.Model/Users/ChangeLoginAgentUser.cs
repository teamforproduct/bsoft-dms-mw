using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class ChangeLoginAgentUser
    {
        [Required]
        public int AgentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NewEmail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsVerificationRequired { get; set; }
    }
}
