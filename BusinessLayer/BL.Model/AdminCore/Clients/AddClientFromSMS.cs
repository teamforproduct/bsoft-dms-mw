using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.Clients
{
    /// <summary>
    /// Модель для добавления нового клиента
    /// </summary>
    public class AddClientFromSMS
    {
        [Required]
        public string SMSCode { get; set ; }

    }
}