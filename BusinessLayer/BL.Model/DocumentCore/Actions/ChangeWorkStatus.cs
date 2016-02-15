using BL.Model.Users;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для окончания/возобновления работы с документом
    /// </summary>
    public class ChangeWorkStatus : CurrentPosition
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        [Required]
        public int DocumentId { get; set; }
        /// <summary>
        /// Признак не в работе / в работе
        /// </summary>
        [Required]
        public bool IsInWork { get; set; }
        /// <summary>
        /// Комментарий к действию
        /// </summary>
        [Required]
        public string Description { get; set; }
    }
}
