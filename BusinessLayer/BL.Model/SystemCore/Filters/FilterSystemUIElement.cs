using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.SystemCore.Filters
{
    public class FilterSystemUIElement
    {
        public List<int> UIElementId { get; set; }
        public List<int> ActionId { get; set; }
        public List<int> ObjectId { get; set; }
        public string Code { get; set; }
        public string ActionCode { get; set; }
        public string ObjectCode { get; set; }

        


    }
}
