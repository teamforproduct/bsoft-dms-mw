using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.SystemCore
{
    public class FilterSystemAction
    {
        public List<int> Id { get; set; }
        public string Code { get; set; }
        public string ObjectCode { get; set; }
        public bool? IsAvailabel { get; set; }


    }
}
