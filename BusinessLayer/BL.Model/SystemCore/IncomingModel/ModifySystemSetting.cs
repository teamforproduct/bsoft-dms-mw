using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.IncomingModel
{
    public class ModifySystemSetting
    {
        [IgnoreDataMember]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }

        public int? AgentId { get; set; }
    }
}
