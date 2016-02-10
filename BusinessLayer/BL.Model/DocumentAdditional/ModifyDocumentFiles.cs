using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DocumentAdditional
{
    public class ModifyDocumentFiles
    {
        public int DocumentId { get; set; }
        public IEnumerable<ModifyDocumentFile> Files { get; set; }
    }
}
