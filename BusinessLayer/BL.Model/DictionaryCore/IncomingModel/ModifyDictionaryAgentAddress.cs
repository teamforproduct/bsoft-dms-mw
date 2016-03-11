using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace BL.Model.DictionaryCore.IncomingModel
{
    public class ModifyDictionaryAgentAddress
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int AddressTypeId { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
