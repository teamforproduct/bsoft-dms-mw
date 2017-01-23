using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для модификации бумажных носителей
    /// </summary>
    public class ModifyDocumentPapers: BaseModifyDocumentPapers
    {
        /// <summary>
        /// ИД Бумажного носителя
        /// </summary>
        public int Id { get; set; }
    }
}
