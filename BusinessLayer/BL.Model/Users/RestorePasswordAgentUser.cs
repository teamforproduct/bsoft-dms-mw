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
        /// <summary>
        /// 
        /// </summary>       
        [Required]
        public string Email { get; set; }


        /// <summary>
        /// 
        /// </summary>   
        [Required]
        public string ClientCode { get; set; }
        
        public string FirstEntry { get; set; }

    }
}
