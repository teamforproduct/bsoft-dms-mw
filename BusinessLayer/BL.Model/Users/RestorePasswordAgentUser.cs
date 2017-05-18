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

        /// <summary>
        /// Код языка ru_Ru
        /// </summary>
        [Required]
        public string LanguageCode { get; set; }

        public string FirstEntry { get; set; }

    }
}
