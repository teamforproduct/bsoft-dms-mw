using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Базовая модель для добаления/исправления задачи
    /// </summary>
    public class BaseModifyDocumentTasks
    {
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
    }
}
