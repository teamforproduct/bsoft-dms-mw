using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore
{
    public class DocumentAccess
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int PositionId { get; set; }
        public int AccessLevelId { get; set; }
        public bool IsInWork { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
