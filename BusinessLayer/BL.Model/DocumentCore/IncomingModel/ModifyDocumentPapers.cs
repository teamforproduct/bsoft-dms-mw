using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для модификации бумажных носителей
    /// </summary>
    public class ModifyDocumentPapers: BaseModifyDocumentPapers
    {
        public ModifyDocumentPapers()
        {

        }
        public ModifyDocumentPapers (AddDocumentPapers model)
        {
            DocumentId = model.DocumentId;
            Name = model.Name;
            Description = model.Description;
            IsMain = model.IsMain;
            IsOriginal = model.IsOriginal;
            IsCopy = model.IsCopy;
            PageQuantity = model.PageQuantity;
            PaperQuantity = model.PaperQuantity;
        }
        /// <summary>
        /// ИД Бумажного носителя
        /// </summary>
        public int Id { get; set; }
    }
}
