using System.ComponentModel.DataAnnotations;
using BL.Model.Users;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель события jтметить нахождение бумажного носителя у себя
    /// </summary>
    public class MarkOwnerDocumentPaper: PaperEvent
    {
        /// <summary>
        /// ИД должности, от которой будет выполнятся действие
        /// </summary>
        [Required]
        public int CurrentPositionId { get; set; }
    }
}
