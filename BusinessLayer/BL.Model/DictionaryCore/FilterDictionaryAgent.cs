using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore
{
    public class FilterDictionaryAgent
    {
        public List<int> Id { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsEmployee { get; set; }
    }
}
