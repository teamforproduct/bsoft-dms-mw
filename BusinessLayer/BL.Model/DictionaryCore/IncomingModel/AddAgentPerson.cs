using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контрагент - физическое лицо
    /// </summary>
    public class AddAgentPerson : AddAgentPeople
    {
        /// <summary>
        /// Id компании, контактным лицом которой является физическое лицо
        /// </summary>
        public int? AgentCompanyId { get; set; }

        /// <summary>
        /// Должность (текстопое поле)
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///Признак активности
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [IgnoreDataMember]
        public new string Name { get; set; }

    }
}
