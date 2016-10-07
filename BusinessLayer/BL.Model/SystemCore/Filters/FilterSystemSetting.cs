using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.SystemCore.Filters
{
    public class FilterSystemSetting
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int? AgentId { get; set; }
    }
}