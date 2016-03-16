using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterCustomDictionary
    {
        public List<int> CustomDictionaryTypeId { get; set; }
        public string Code { get; set; }
        public List<int> NotContainsId { get; set; }
    }
}
