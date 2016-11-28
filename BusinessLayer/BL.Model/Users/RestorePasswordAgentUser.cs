using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Users
{
    public class RestorePasswordAgentUser
    {
        [Required]
        /// <summary>
        /// 
        /// </summary>       
        public string Email { get; set; }

        [Required]
        /// <summary>
        /// 
        /// </summary>       
        public string ClientCode { get; set; }
    }
}
