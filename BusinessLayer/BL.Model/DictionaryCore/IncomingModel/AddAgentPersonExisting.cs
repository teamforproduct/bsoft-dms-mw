using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class AddAgentPersonExisting
    {
        /// <summary>
        /// Id компании, контактным лицом которой является физическое лицо
        /// </summary>
        [Required]
        public int CompanyId { get; set; }


        /// <summary>
        /// Id физического лица
        /// </summary>
        [Required]
        public int PersonId { get; set; }

    }
}
