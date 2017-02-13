using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.IncomingModel
{
    public class SetJournalAccess
    {

        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Id Должности
        /// </summary>
        [Required]
        public int PositionId { get; set; }

        /// <summary>
        /// Id журнала регистрации
        /// </summary>
        [Required]
        public int RegistrationJournalId { get; set; }

        /// <summary>
        /// Тип доступа к журналу
        /// </summary>
        [Required]
        public EnumRegistrationJournalAccessTypes RegJournalAccessTypeId { get; set; }

        /// <summary>
        /// Установить галочку
        /// </summary>
        [Required]
        public bool IsChecked { get; set; }

    }
}