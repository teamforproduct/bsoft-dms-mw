using System.Collections.Generic;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentSendListStage
    {
        public int Stage { get; set; }
        public IEnumerable<FrontDocumentSendList> SendLists { get; set; }
        public bool IsOpen { get; set; }
        public bool IsClose { get; set; }
    }
}
