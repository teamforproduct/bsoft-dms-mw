using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.IncomingModel
{
    public class SetJournalAccessByCompany_Position
    {

        /// <summary>
        /// Id Должности
        /// </summary>
        [Required]
        public int PositionId { get; set; }

        /// <summary>
        /// Компания
        /// </summary>
        [Required]
        public int CompanyId { get; set; }

        /// <summary>
        /// Тип рассылки (для исполнения, для сведения)
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