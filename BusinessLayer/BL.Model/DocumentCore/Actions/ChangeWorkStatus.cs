using BL.Model.Users;

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
        public int DocumentId { get; set; }
        /// <summary>
        /// Признак не в работе / в работе
        /// </summary>
        public bool IsInWork { get; set; }
        /// <summary>
        /// Комментарий к действию
        /// </summary>
        public string Description { get; set; }
    }
}
