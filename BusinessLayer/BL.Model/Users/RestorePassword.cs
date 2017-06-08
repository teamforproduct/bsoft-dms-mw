using System.ComponentModel.DataAnnotations;

namespace BL.Model.Users
{
    public class RestorePassword
    {
        /// <summary>
        /// 
        /// </summary>       
        [Required]
        public string Email { get; set; }


        /// <summary>
        /// 
        /// </summary>   
        public string ClientCode { get; set; }
    }
}
