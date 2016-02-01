using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentRestrictedSendList
    {
        public int DocumentId { get; set; }
        public Nullable<int> PositionId { get; set; }
        public int AccessLevelId { get; set; }
    }
}
