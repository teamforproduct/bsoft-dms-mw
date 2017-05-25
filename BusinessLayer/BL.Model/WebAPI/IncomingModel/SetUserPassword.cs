using System.ComponentModel.DataAnnotations;

namespace BL.Model.WebAPI.IncomingModel
{
    public class SetUserPassword
    {
        /// <summary>
        /// Новый пароль
        /// </summary>
        [Required]
        public string NewPassword { get; set; }
    }

}