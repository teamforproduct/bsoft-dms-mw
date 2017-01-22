using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для исправления задачи
    /// </summary>
    public class ModifyDocumentTasks : BaseModifyDocumentTasks
    {
        public ModifyDocumentTasks(AddDocumentTasks model)
        {
            DocumentId = model.DocumentId;
            Name = model.Name;
            Description = model.Description;
        }
        /// <summary>
        /// ИД Task
        /// </summary>
        public int Id { get; set; }
    }
}
