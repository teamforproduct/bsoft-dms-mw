using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.Clients
{
    /// <summary>
    /// Модель для добавления нового клиента
    /// </summary>
    public class AddClientFromHash
    {
        [Required]
        public string Hash { get; set ; }

        public string Password { get; set; }

    }
}