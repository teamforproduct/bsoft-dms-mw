using System.ComponentModel.DataAnnotations;

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
