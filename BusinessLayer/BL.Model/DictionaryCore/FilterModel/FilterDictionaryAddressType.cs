using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryAddressType
    {
        public List<int> AddressTypeId { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public List<int> NotContainsId { get; set; }
    }
}
