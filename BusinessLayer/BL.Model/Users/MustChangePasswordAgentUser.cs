using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class MustChangePasswordAgentUser
    {
        [Required]
        public int AgentId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool MustChangePassword { get; set; }
    }
}
