using BL.Model.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontSystemSetting
    {
        public string Key { get; set; }
        
        public object Value { get; set; }

        [IgnoreDataMember]
        public EnumValueTypes ValueType { get; set; }

        public int? AgentId { get; set; }
    }
}
