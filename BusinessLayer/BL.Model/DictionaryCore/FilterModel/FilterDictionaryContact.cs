using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryContact
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int ContactTypeId { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
}
