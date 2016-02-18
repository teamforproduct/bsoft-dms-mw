using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore
{
    /// <summary>
    /// Модель для модификации плана работы над документом
    /// </summary>
    public class ModifyDocumentSendListStage
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Этап
        /// </summary>
        [Required]
        public int Stage { get; set; }
    }
}
