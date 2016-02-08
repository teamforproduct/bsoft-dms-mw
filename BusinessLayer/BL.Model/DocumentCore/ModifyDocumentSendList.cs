using System;
using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentSendList
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int Stage { get; set; }
        public EnumSendType SendType { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public int? DueDay { get; set; }
        public EnumDocumentAccess AccessLevel { get; set; }
    }
}
