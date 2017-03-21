using System.ComponentModel.DataAnnotations;

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
