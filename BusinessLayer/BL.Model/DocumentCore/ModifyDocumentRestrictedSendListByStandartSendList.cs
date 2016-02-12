using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    public class ModifyDocumentRestrictedSendListByStandartSendList : CurrentPosition
    {
        public int StandartSendListId { get; set; }
        public int DocumentId { get; set; }
    }
}
