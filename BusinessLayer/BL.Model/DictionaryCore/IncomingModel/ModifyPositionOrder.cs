using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class ModifyPositionOrder
    {
        /// <summary>
        /// Должность
        /// </summary>
        [Required]
        public int PositionId { get; set; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        [Required]
        public int Order { get; set; }

    }
}