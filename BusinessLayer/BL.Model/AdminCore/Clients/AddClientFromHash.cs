using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.Clients
{
    /// <summary>
    /// Модель для добавления нового клиента
    /// </summary>
    public class AddClientFromHash
    {
        [Required]
        public string Hash { get; set ; }

    }
}