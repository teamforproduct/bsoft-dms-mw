using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterCustomDictionary : BaseFilterCodeNameIsActive
    {
        [IgnoreDataMember]
        public int? TypeId { get; set; }
    }
}
