using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class ModifyDictionaryStandartSendList :LastChangeInfo
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PositionId { get; set; }
        
    }
}
