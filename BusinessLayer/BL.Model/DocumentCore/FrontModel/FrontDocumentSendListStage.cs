using System.Collections.Generic;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentSendListStage
    {
        public int Stage { get; set; }
        public IEnumerable<FrontDocumentSendList> SendLists { get; set; }
    }
}
