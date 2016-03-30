using BL.Model.Users;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель создания копий бумажных носителей
    /// </summary>
    public class CopyDocumentPaper: EventPaper
    {
        /// <summary>
        /// Имя копии
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Количество копий
        /// </summary>
        public int Quantity { get; set; }
    }
}
