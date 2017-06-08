using System.ComponentModel.DataAnnotations;

namespace BL.Model.Users
{
    public class ChangeLockoutAgentUser
    {
        /// <summary>
        /// Id сотрудника - пользователя
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// блокирование/разблокирование сотрудника
        /// </summary>
        [Required]
        public bool IsLockout { get; set; }

    }
}
