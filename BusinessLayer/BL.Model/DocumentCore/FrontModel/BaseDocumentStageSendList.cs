using System.Collections.Generic;

namespace BL.Model.DocumentCore.FrontModel
{
    public class BaseDocumentSendListStage
    {
        public int Stage { get; set; }
        public IEnumerable<BaseDocumentSendList> SendLists { get; set; }
    }
}
