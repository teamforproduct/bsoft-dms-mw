using BL.Model.Common;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterCustomDictionary : BaseFilterCodeNameIsActive
    {
        public List<int> TypeIDs { get; set; }
    }
}
