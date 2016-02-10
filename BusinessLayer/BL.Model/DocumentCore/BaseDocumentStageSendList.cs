using System;
using System.Collections.Generic;

namespace BL.Model.DocumentCore
{
    public class BaseDocumentSendListStage
    {
        public int Stage { get; set; }
        public IEnumerable<BaseDocumentSendList> SendLists { get; set; }
    }
}
