using BL.Model.Users;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления списка файлов к документу
    /// </summary>
    public class ModifyDocumentFiles: CurrentPosition
    {
        /// <summary>
        /// ИД. документа, к которому добавляются файлы
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Список добавляемых файлов
        /// </summary>
        public IEnumerable<ModifyDocumentFile> Files { get; set; }
    }
}
