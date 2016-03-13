using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryContactType
    {
        public List<int> ContactTypeId { get; set; }
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
