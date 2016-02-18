using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления плана работы над документом по стандартному списку
    /// </summary>
    public class ModifyDocumentSendListByStandartSendList
    {
        /// <summary>
        /// ИД стандартного списка
        /// </summary>
        [Required]
        public int StandartSendListId { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
    }
}
