using System.Runtime.Serialization;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentFile
    {
        public int DocumentId { get; set; }
        public bool IsAdditional { get; set; }
        public string FileData { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }
}
