using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.IncomingModel
{
    public class SetJournalAccessByCompany_Journal
    {

        /// <summary>
        /// Id Журнала
        /// </summary>
        [Required]
        public int JournalId { get; set; }

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