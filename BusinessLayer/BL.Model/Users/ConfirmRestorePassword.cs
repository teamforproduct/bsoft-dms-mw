using System.ComponentModel.DataAnnotations;

namespace BL.Model.Users
{
    public class ConfirmRestorePassword
    {

        /// <summary>
        /// 
        /// </summary>       
        [Required]
        public string UserId { get; set; }


        /// <summary>
        /// 
        /// </summary>       
        [Required]
        public string Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
