using BL.Model.Users;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для добавления примечания
    /// </summary>
    public class AddNote : CurrentPosition
    {
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        public string Description { get; set; }
     
    }
}
