using System.Collections.Generic;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentSendListStage
    {
        public int Stage { get; set; }
        public IEnumerable<InternalDocumentSendList> SendLists { get; set; }
    }
}