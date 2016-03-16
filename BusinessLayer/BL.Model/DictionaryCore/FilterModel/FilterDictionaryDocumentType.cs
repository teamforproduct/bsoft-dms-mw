using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryDocumentType
    {
        public List<int> DocumentTypeId { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public List<int> NotContainsId { get; set; }
    }
}
