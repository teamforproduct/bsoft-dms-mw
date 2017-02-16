using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.IncomingModel
{
    public class SetJournalAccessDefault_Journal
    {

        /// <summary>
        /// Id журнала
        /// </summary>
        [Required]
        public int JournalId { get; set; }

    }
}