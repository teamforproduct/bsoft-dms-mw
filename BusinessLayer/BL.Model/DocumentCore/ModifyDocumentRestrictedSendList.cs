using System;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentRestrictedSendList
    {
        public int DocumentId { get; set; }
        public Nullable<int> PositionId { get; set; }
        public int AccessLevelId { get; set; }
    }
}
