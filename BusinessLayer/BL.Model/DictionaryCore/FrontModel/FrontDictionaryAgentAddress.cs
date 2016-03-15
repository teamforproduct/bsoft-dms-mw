using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontDictionaryAgentAddress
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public FrontDictionaryAddressType AddressType { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
