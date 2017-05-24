using System.ComponentModel.DataAnnotations;

namespace BL.Model.Users
{
    public class MustChangePasswordAgentUser
    {
        [Required]
        public int Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool MustChangePassword { get; set; }
    }
}
