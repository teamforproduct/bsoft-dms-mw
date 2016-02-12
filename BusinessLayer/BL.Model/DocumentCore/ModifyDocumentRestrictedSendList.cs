using BL.Model.Users;
using System;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentRestrictedSendList
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Nullable<int> PositionId { get; set; }
        public int AccessLevelId { get; set; }
    }
}
