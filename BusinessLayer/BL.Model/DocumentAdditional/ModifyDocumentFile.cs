using System.Runtime.Serialization;

namespace BL.Model.DocumentAdditional
{
    public class ModifyDocumentFile
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int OrderInDocument { get; set; }
        public bool IsAdditional { get; set; }
        public string FileData { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }
}
