using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для исправления задачи
    /// </summary>
    public class ModifyDocumentTask : BaseModifyDocumentTask
    {
        /// <summary>
        /// ИД Task
        /// </summary>
        public int Id { get; set; }
    }
}
