using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.WebUser
{
    /// <summary>
    /// Модель для добавления нового аккаунта
    /// </summary>
    public class AddWebUser
    {
        /// <summary>
        /// Id клиента
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// ServerId
        /// </summary>
        public int ServerId { get; set; }

        /// <summary>
        /// Login
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }


    }
}