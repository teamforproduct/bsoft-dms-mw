using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования
    /// </summary>
    public class AddAddressType
    {
        /// <summary>
        /// Краткое наименование типа адреса.
        /// </summary>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// Название типа адреса. Отображается в документе
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}
