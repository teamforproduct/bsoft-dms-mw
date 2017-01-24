using BL.Model.Users;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления бумажного носителя
    /// </summary>
    public class AddDocumentPapers : BaseModifyDocumentPapers
    {
        /// <summary>
        /// ИД должности, от которой будет выполнятся действие
        /// </summary>
        [Required]
        public int CurrentPositionId { get; set; }

    }
}
