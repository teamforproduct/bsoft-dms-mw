using System;
using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentFile
    {
        [IgnoreDataMember]
        public int DocumentId { get; set; }
        public bool isAdditional { get; set; }
    }
}
