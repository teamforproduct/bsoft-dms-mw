using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.SystemCore.Filters
{
    public class FilterSystemSetting
    {
        /// <summary>
        /// Список ID
        /// </summary>
        public List<int> IDs { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int? AgentId { get; set; }
    }
}