using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.AdminCore
{
    public class VerifyAccess
    {
        public int UserId { get; set; }
        public string ObjectCode { get; set; }
        public string ActionCode { get; set; }
        public int? RecordId { get; set; }
        public int? PositionId { get; set; }
        public List<int> PositionsIdList { get; set; }
    }
}
