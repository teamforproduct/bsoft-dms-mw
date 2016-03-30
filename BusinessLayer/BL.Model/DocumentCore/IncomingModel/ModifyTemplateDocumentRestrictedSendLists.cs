using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class ModifyTemplateDocumentRestrictedSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }
        public EnumDocumentAccesses AccessLevel { get; set; }
    }
}
