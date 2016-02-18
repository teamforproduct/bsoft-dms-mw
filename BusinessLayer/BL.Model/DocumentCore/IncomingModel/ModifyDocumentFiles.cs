using System.Collections.Generic;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class ModifyDocumentFiles
    {
        public int DocumentId { get; set; }
        public IEnumerable<ModifyDocumentFile> Files { get; set; }
    }
}
