using System;
using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel 
{
    public class InternalDictionaryAgentAddress : LastChangeInfo
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int AddressTypeID { get; set; }
        public string PostCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
