using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryAgentAddress
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public List<int> AddressTypeId { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public bool? IsActive { get; set; }
    }
}
