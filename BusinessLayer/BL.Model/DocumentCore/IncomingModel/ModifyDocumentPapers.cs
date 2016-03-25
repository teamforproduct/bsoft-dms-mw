using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для перезаписи списка тегов документа
    /// </summary>
    public class ModifyDocumentPapers : CurrentPosition
    {
        /// <summary>
        /// ИД Task
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public bool IsOriginal { get; set; }
        public bool IsCopy { get; set; }
        public int PageQuantity { get; set; }
        public int OrderNumber { get; set; }
        public int LastPaperEventId { get; set; }
    }
}
