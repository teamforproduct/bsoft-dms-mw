using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.FrontModel
{
    public class FrontSystemSetting
    {
        public int Id { get; set; }

        public string Key { get; set; }
        
        public string Value { get; set; }

        public int? AgentId { get; set; }
    }
}
