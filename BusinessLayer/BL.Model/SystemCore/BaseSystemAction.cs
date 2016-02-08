using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.SystemCore
{
    public class BaseSystemAction
    {
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public string Code { get; set; }
        public string API { get; set; }
        public string Description { get; set; }
        public bool IsGrantable { get; set; }
        public bool IsGrantableByRecordId { get; set; }

        public string ObjectName { get; set; }

    }
}
