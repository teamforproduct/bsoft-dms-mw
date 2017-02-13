using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// Дублирование настройки рассылки для должностей
    /// </summary>
    public class DuplicateJournalAccess
    {

        /// <summary>
        /// Журнал, с которого скопировать доступы 
        /// </summary>
        [Required]
        public int SourceJournalId { get; set; }

        /// <summary>
        /// Журнал, которому применить доступы 
        /// </summary>
        [Required]
        public int TargetJournalId { get; set; }

        /// <summary>
        /// Режим копирования
        /// </summary>
        [Required]
        public EnumCopyMode CopyMode { get; set; }

    }
}